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
        SceneController.restartLvl += Hide;
    }

    private void ShowOnPlayerDeath()
    {
        anim.Play("showPopup");
    }

    public void Hide()
    {
        anim.Play("hidePopup");
    }

    void OnDestroy()
    {
        playerHealth.onPersonDead -= ShowOnPlayerDeath;
        SceneController.restartLvl -= Hide;
    }
}
