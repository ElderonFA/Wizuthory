using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private RectTransform healthIndicator;
    [SerializeField] private Image blackFon;
    [SerializeField] private CutSceneController cutSceneController;
    [SerializeField] private DeathPopup deathPopup;

    public static Action startLevelEvent;
    public static Action endLevelEvent;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartHideBlackScreen();
        blackFon.gameObject.SetActive(true);
        cutSceneController.gameObject.SetActive(true);
        
        startLevelEvent += StartHideBlackScreen;
        endLevelEvent += StartShowingBlackScreen;
        
        SceneController.exitToMenu += DestroySelf;
        SceneController.restartLvl += HideDeathPopup;
        SceneController.restartLvl += UpdateBar;
        SceneController.actionGetInterfaceController?.Invoke(this);
        
        health.onPersonTakeDamage += UpdateBar;
    }

    private void HideDeathPopup()
    {
        deathPopup.Hide();
    }

    private void UpdateBar()
    {
        var width = health.CurrentHp / health.MaxHp;
        healthIndicator.localScale = new Vector3(width, 1f, 1f);
    }

    private void StartShowingBlackScreen()
    {
        StartCoroutine(ShowBlackScreen());
    }
    
    private void StartHideBlackScreen()
    {
        StartCoroutine(HideBlackScreen());
    }
    
    private IEnumerator ShowBlackScreen()
    {
        var startColor = blackFon.color;
        var alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            
            blackFon.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
    }
    
    private IEnumerator HideBlackScreen()
    {
        var startColor = blackFon.color;
        var alpha = 1f;
        while (alpha >= 0)
        {
            alpha -= Time.deltaTime;
            
            blackFon.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneController.exitToMenu -= DestroySelf;
        SceneController.restartLvl -= HideDeathPopup;
        SceneController.restartLvl -= UpdateBar;
        
        startLevelEvent -= StartHideBlackScreen;
        endLevelEvent -= StartShowingBlackScreen;
        
        health.onPersonTakeDamage -= UpdateBar;
    }
}
