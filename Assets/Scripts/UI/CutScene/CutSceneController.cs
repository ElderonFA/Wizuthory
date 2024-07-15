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
    [Space] 
    [SerializeField] private Image skipView;

    [SerializeField] 
    private CutSceneConfig finalCutScene;
    
    public static Action<CutScene> OnStartCutScene;
    
    private bool endLevel;
    private bool blackFonWasShow;
    private bool waitForBlackScreen;
    public bool cutSceneIsEnd;

    private bool bordersIsShow;

    private bool skipCutScenes = false;

    private BossController bossController;

    public Action onEndGame;

    private float skipCutSceneTimer;
    private float timeToHoldForSkipCutScene = 1f;
    
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
        SceneController.actionGetCutSceneController?.Invoke(this);
        
        OnStartCutScene += StartCutSceneCoroutine;
        HealthBarController.onBlackFonShowed += SetBlackFonWasShowTrue;
        HealthBarController.onBlackFonHide += SetBlackFonWasShowFalse;
        onEndGame += StartFinalCutScene;
    }

    private void SetBlackFonWasShowFalse()
    {
        blackFonWasShow = false;
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
            if (Input.GetKey(KeyCode.Tab))
            {
                skipCutSceneTimer += Time.deltaTime * 2f;
                skipView.fillAmount = timeToHoldForSkipCutScene * skipCutSceneTimer;
                
                if (skipCutSceneTimer >= timeToHoldForSkipCutScene)
                {
                    playerController.SetCanMove(true);
                    cutSceneIsEnd = true;
                    waitForBlackScreen = false;
                    skipCutSceneTimer = 0f;

                    foreach (var cutSceneStep in allStep)
                    {
                        ShowStep(cutSceneStep, leftImage);

                        if (cutSceneStep.GetEvent == CutSceneEvents.EndLevel)
                        {
                            SceneController.toNewLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
                            endLevel = false;
                        }
                    }
                    
                    StartCoroutine(HideBordersAnim());
                    clickToContinue.color = new Color(1f, 1f, 1f, 0f);

                    while (bordersIsShow)
                    {
                        yield return null;
                    }
                    
                    yield break;
                }
            }
            else
            {
                skipCutSceneTimer = 0f;
                skipView.fillAmount = 0f;
            }

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
                    //Переход между уровнями в середине катсцен
                    if (endLevel)
                    {
                        HealthBarController.endLevelEvent?.Invoke();
            
                        while (!blackFonWasShow)
                        {
                            yield return null;
                        }
            
                        SceneController.toNewLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
                        endLevel = false;
                    }
                    
                    currentStepIdx++;
                    ShowStep(allStep[currentStepIdx], leftImage);
                    textShowDelay = startDelay;
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
            yield return null;
        }
        
        StartCoroutine(HideBordersAnim());
        clickToContinue.color = new Color(1f, 1f, 1f, 0f);

        while (bordersIsShow)
        {
            yield return null;
        }
        
        if (endLevel)
        {
            HealthBarController.endLevelEvent?.Invoke();
            
            while (!blackFonWasShow)
            {
                yield return null;
            }
            
            SceneController.toNewLevel?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
            endLevel = false;
        }
        
        playerController.SetCanMove(true);
        cutSceneIsEnd = true;
        waitForBlackScreen = false;
    }
    
    private void ShowStep(CutSceneStep cutSceneStep, Image imagePlace)
    {
        textField.text = cutSceneStep.GetText;

        var currentPerson = personsConfigs.First(x => x.personType == cutSceneStep.GetPerson);

        switch (currentPerson.personType)
        {
            case Persons.Player:
                cinemachineVirtualCamera.Follow = currentPerson.personPosition;
                break;
            case Persons.Skeleton:
                var skeleton = GameObject.Find("SkeletonFromCutScene");
            
                var skeletonAnim = skeleton.GetComponent<Animator>();
                if (skeletonAnim.enabled == false)
                {
                    skeletonAnim.enabled = true;
                }

                cinemachineVirtualCamera.Follow = skeleton.transform;
                break;
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
