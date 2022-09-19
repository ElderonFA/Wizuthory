using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_seeker_spawn : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private NpcMoving npcMoving;
    [SerializeField] private Health selfHealth;

    [SerializeField] private GameObject attackBlock;

    void OnTriggerEnter2D(Collider2D other)
    {
        selfHealth.onPersonDead += DisableMovement;
        if (other.tag == "Player" && anim.enabled == false && selfHealth.IsAlive)
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

    private void DisableMovement()
    {
        var selfScript = gameObject.GetComponent<Skeleton_seeker_spawn>();
        Destroy(selfScript);
        
        Destroy(attackBlock);
    }
}
