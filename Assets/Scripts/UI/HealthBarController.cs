using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private RectTransform healthIndicator;

    void Start()
    {
        health.onPersonTakeDamage += UpdateBar;
    }

    private void UpdateBar()
    {
        var width = health.CurrentHp / health.MaxHp;
        healthIndicator.localScale = new Vector3(width, 1f, 1f);
    }

    void OnDestroy()
    {
        health.onPersonTakeDamage -= UpdateBar;
    }
}
