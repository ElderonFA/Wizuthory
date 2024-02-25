using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private RectTransform healthIndicator;

    void Start()
    {
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
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneController.exitToMenu -= DestroySelf;
        PlayerController.RestartLvl -= DestroySelf;
        
        health.onPersonTakeDamage -= UpdateBar;
    }
}
