using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private RectTransform healthIndicator;
    [SerializeField] private Image blackFon;

    public static Action startLevelEvent;
    public static Action endLevelEvent;

    void Start()
    {
        StartHideBlackScreen();
        
        startLevelEvent += StartHideBlackScreen;
        endLevelEvent += StartShowingBlackScreen;
        
        SceneController.exitToMenu += DestroySelf;
        PlayerController.RestartLvl += DestroySelf;
        
        health.onPersonTakeDamage += UpdateBar;
        
        DontDestroyOnLoad(gameObject);
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
        PlayerController.RestartLvl -= DestroySelf;
        
        startLevelEvent += StartHideBlackScreen;
        endLevelEvent -= StartShowingBlackScreen;
        
        health.onPersonTakeDamage -= UpdateBar;
    }
}
