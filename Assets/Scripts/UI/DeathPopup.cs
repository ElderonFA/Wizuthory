using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPopup : MonoBehaviour
{
    [SerializeField] private Health playerHealth;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        playerHealth.onPersonDead += ShowOnPlayerDeath;
    }

    private void ShowOnPlayerDeath()
    {
        if (!anim.enabled)
            anim.enabled = true;
    }

    void OnDestroy()
    {
        playerHealth.onPersonDead -= ShowOnPlayerDeath;
    }
}
