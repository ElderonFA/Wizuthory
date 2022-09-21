using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Skeleton_seeker_spawn : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private NpcMoving npcMoving;
    [SerializeField] private Health selfHealth;

    [SerializeField] private GameObject attackBlock;

    [SerializeField] private SpriteRenderer selfView;

    public void Update()
    {
        Debug.Log(selfHealth.IsAlive);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && anim.enabled == false && selfHealth.IsAlive)
        {
            anim.enabled = true;
        }
    }

    public void NullAnim(string name)
    {
        anim.SetBool(name, false);
    }

    public void StartDead()
    {
        anim.enabled = false;
        selfHealth.KillSelf();
    }

    public void SetCanMoveTrue()
    {
        npcMoving.SetCanMove();
    }
}
