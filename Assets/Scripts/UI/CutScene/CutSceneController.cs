using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CutSceneController : MonoBehaviour
{
    [Header("Camera")] 
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    [Header("Player")] 
    [SerializeField] private PlayerController playerController;
    
    [Space]
    [SerializeField] private RectTransform topBorder;
    [SerializeField] private RectTransform bottomBorder;

    [SerializeField] private Text textField;

    [Header("Delays")]
    [SerializeField] private float textShowDelay;

    [SerializeField] private Text clickToContinue;

    [Header("Person images")] 
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [Space] 
    [SerializeField] private List<PersonInCutscenes> personsConfigs;

    [SerializeField] 
    private CutSceneConfig finalCutScene;
    
    public static Action<CutScene> OnStartCutScene;
    
    private bool endLevel;
    private bool blackFonWasShow;
    private bool waitForBlackScreen;
    private bool cutSceneIsEnd;

    private bool bordersIsShow;

    private bool skipCutScenes = false;

    private BossController bossController;

    public Action onEndGame;
    
    [Serializable]
    public class PersonInCutscenes
    {
        public Persons personType;
        public Sprite personIcon;
        public Transform personPosition;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        OnStartCutScene += StartCutSceneCoroutine;
        HealthBarController.onBlackFonShowed += SetBlackFonWasShowTrue;
        onEndGame += StartFinalCutScene;
    }

    private void SetBlackFonWasShowTrue()
    {
        blackFonWasShow = true;
    }

    private void StartCutSceneCoroutine(CutScene cutScene)
    {
        if (playerController.isDebug && skipCutScenes)
        {
            return;
        }

        StartCoroutine(StartCutScene(cutScene));
    }

    private IEnumerator StartCutScene(CutScene cutScene)
    {
        cutSceneIsEnd = false;
        playerController.SetCanMove(false);
        var startDelay = textShowDelay;
        
        var currentStepIdx = 0;
        var allStep = cutScene.GetAllStep;
        
        ShowStep(allStep[currentStepIdx], leftImage);
        
        StartCoroutine(ShowBordersAnim());
        while (!bordersIsShow)
        {
            yield return null;
        }

        while (currentStepIdx < allStep.Length - 1)
        {
            if (textShowDelay > 0)
            {
                textShowDelay -= Time.deltaTime;
            }
            else
            {
                if (waitForBlackScreen)
                {
                    waitForBlackScreen = blackFonWasShow;
                    yield return null;
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    currentStepIdx++;
                    ShowStep(allStep[currentStepIdx], leftImage);
                    textShowDelay = startDelay;
                    
                    if (endLevel)
                    {
                        SceneController.toNewLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
                        endLevel = false;
                    }
                }
            }
            
            yield return null;
        }

        var alph = 0f;
        while (alph < 1f)
        {
            alph += Time.deltaTime;
            clickToContinue.color = new Color(1f, 1f, 1f, alph);
            yield return null;
        }

        while (!Input.GetMouseButtonDown(0))
        {
            if (endLevel && blackFonWasShow)
            {
                HealthBarController.endLevelEvent?.Invoke();
            }
            
            yield return null;
        }
        
        StartCoroutine(HideBordersAnim());
        clickToContinue.color = new Color(1f, 1f, 1f, 0f);
        playerController.SetCanMove(true);

        while (bordersIsShow)
        {
            yield return null;
        }
        
        cutSceneIsEnd = true;
        blackFonWasShow = false;
        waitForBlackScreen = false;
    }
    
    private void ShowStep(CutSceneStep cutSceneStep, Image imagePlace)
    {
        textField.text = cutSceneStep.GetText;

        var currentPerson = personsConfigs.First(x => x.personType == cutSceneStep.GetPerson);

        if (currentPerson.personType == Persons.Skeleton)
        {
            var skeletonAnim = currentPerson.personPosition.gameObject.GetComponent<Animator>();
            if (skeletonAnim.enabled == false)
            {
                skeletonAnim.enabled = true;
            }
        }

        var sprite = currentPerson.personIcon;
        if (sprite)
        {
            imagePlace.color = new Color(1f, 1f, 1f, 1f);
            imagePlace.sprite = sprite;
        }
        else
        {
            imagePlace.color = new Color(1f, 1f, 1f, 0f);
        }

        var target = currentPerson.personPosition;
        if (target)
        {
            cinemachineVirtualCamera.Follow = target;
        }

        var sceneEvent = cutSceneStep.GetEvent;
        if (sceneEvent != CutSceneEvents.None)
        {
            if (sceneEvent == CutSceneEvents.WakeUpBoss
            || sceneEvent == CutSceneEvents.StartUpdateBoss)
            {
                var bossObj = GameObject.Find("Boss");
                bossController = bossObj.GetComponent<BossController>();
            }
            
            switch (sceneEvent)
            {
                case CutSceneEvents.EndLevel:
                    endLevel = true;
                    break;
                
                case CutSceneEvents.BranchCrack:
                    var trees = FindObjectsOfType<TreeWithBranch>();
                    foreach (var tree in trees)
                    {
                        if (tree.gameObject.tag == "CutSceneObject")
                        {
                            tree.DropBranch();
                            break;
                        }
                    }
                    break;
                
                case CutSceneEvents.LookAtWhole:
                    var whole = GameObject.Find("WholePos");
                    cinemachineVirtualCamera.Follow = whole.transform;        
                    break;
                
                case CutSceneEvents.StartFiringSpot:
                    var finalCutSceneHelperObj = GameObject.Find("FinalCutSceneHelper");
                    var finalCutSceneHelper = finalCutSceneHelperObj.GetComponent<FinalCutSceneHelper>();

                    finalCutSceneHelper.StartFiringSpot();
                    break;
                
                case CutSceneEvents.WakeUpBoss:
                    bossController.WakeUp();
                    break;
                
                case CutSceneEvents.StartUpdateBoss:
                    bossController.StartUpdateBoss();
                    break;
                
                case CutSceneEvents.WaitForBlackScreen:
                    HealthBarController.showBlackFon?.Invoke();
                    waitForBlackScreen = true;
                    break;
                
                case CutSceneEvents.EndGame:
                    SceneController.onGameEnd?.Invoke();
                    break;;
            }
        }
    }

    private IEnumerator ShowBordersAnim()
    {
        var startPosTop = topBorder.position.y;
        var endPosTop = startPosTop - topBorder.rect.height;
        
        var startPosBottom = bottomBorder.position.y;
        while (topBorder.position.y > endPosTop)
        {
            startPosTop -= Time.deltaTime * 200f;
            startPosBottom += Time.deltaTime * 200f;
            topBorder.position = new Vector2(topBorder.position.x, startPosTop);
            bottomBorder.position = new Vector2(bottomBorder.position.x, startPosBottom);
            yield return null;
        }

        bordersIsShow = true;
        StopCoroutine(ShowBordersAnim());
    }
    
    private IEnumerator HideBordersAnim()
    {
        var startPosTop = topBorder.position.y;
        var endPosTop = startPosTop + topBorder.rect.height;

        leftImage.color = new Color(1f, 1f,1f, 0f);
        rightImage.color = new Color(1f, 1f,1f, 0f);

        var startPosBottom = bottomBorder.position.y;
        while (topBorder.position.y < endPosTop)
        {
            startPosTop += Time.deltaTime * 200f;
            startPosBottom -= Time.deltaTime * 200f;
            topBorder.position = new Vector2(topBorder.position.x, startPosTop);
            bottomBorder.position = new Vector2(bottomBorder.position.x, startPosBottom);
            yield return null;
        }
        
        bordersIsShow = false;
        StopCoroutine(HideBordersAnim());
    }
    
    private void StartFinalCutScene()
    {
        StartCutSceneCoroutine(finalCutScene.GetConfigCutScene);
    }

    public void OnDestroy()
    {
        OnStartCutScene -= StartCutSceneCoroutine;
        onEndGame -= StartFinalCutScene;
    }
}
