using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutSceneController : MonoBehaviour
{
    [Header("Player")] 
    [SerializeField] private PlayerController playerController;
    
    [Space]
    [SerializeField] private RectTransform topBorder;
    [SerializeField] private RectTransform bottomBorder;

    [SerializeField] private Text textField;

    [SerializeField] private float textShowDelay;

    public static Action<CutScene> OnStartCutScene;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        OnStartCutScene += StartCutSceneCoroutine;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Клавиша нажата");
            
            StartCoroutine(StartCutScene(new CutScene(new List<string> {"Hello", "World!", "It is my", "First", "Cut scene"})));
            //OnStartCutScene?.Invoke(new CutScene(new List<string> {"Hello", "World!", "It is my", "First", "Cut scene"}));
        }
    }

    public class CutScene
    {
        private List<string> allTextStep;
        public List<string> GetAllTextStep => allTextStep;

        public CutScene(List<string> newAllText)
        {
            allTextStep = newAllText;
        }
    }

    public void StartCutSceneCoroutine(CutScene cutScene)
    {
        StartCutScene(cutScene);
    }

    private IEnumerator StartCutScene(CutScene cutScene)
    {
        StartCoroutine(ShowBordersAnim());
        playerController.SetCanMove(false);
        var startDelay = textShowDelay;
        
        var currentTextIdx = 0;
        var allText = cutScene.GetAllTextStep;

        textField.text = allText[currentTextIdx];
        
        while (currentTextIdx < allText.Count - 1)
        {
            textShowDelay -= Time.deltaTime;
            
            if (Input.GetMouseButtonDown(0)
            &&  textShowDelay <= 0)
            {
                currentTextIdx++;
                textField.text = allText[currentTextIdx];
                textShowDelay = startDelay;
            }
            yield return null;
        }

        StartCoroutine(HideBordersAnim());
        StopCoroutine(StartCutScene(new CutScene(new List<string>())));
        
        playerController.SetCanMove(false);
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
        StopCoroutine(ShowBordersAnim());
    }
    
    private IEnumerator HideBordersAnim()
    {
        var startPosTop = topBorder.position.y;
        var endPosTop = startPosTop + topBorder.rect.height;
        
        var startPosBottom = bottomBorder.position.y;
        while (topBorder.position.y < endPosTop)
        {
            startPosTop += Time.deltaTime * 200f;
            startPosBottom -= Time.deltaTime * 200f;
            topBorder.position = new Vector2(topBorder.position.x, startPosTop);
            bottomBorder.position = new Vector2(bottomBorder.position.x, startPosBottom);
            yield return null;
        }
        StopCoroutine(HideBordersAnim());
    }
}
