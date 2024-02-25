using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAttack : MonoBehaviour
{
    [SerializeField] private Transform leftSiteObject;
    [SerializeField] private Transform rightSiteObject;
    [SerializeField] private Transform attackObject;
    
    [SerializeField] private Animator anim;
    
    [SerializeField] private AreaEffector2D areaEffector;

    public void ChangeAttackPos(bool left)
    {
        if (left)
        {
            attackObject.transform.position = leftSiteObject.position;
            areaEffector.forceAngle = 210;
        }
        else
        {
            attackObject.transform.position = rightSiteObject.position;
            areaEffector.forceAngle = 30;
        }
        
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("IsAttack", true);
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
