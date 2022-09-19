using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_seeker_spawn : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private NpcMoving npcMoving;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && anim.enabled == false)
        {
            anim.enabled = true;
        }
    }

    public void NullAnim(string name)
    {
        anim.SetBool(name, false);
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    public void SetCanMoveTrue()
    {
        npcMoving.SetCanMove();
    }
}
