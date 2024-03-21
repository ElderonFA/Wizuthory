using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWithBranch : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody2D rbBranch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.tag == "CutSceneObject")
            {
                return;
            }
            
            DropBranch();
        }
    }

    public void DropBranch()
    {
        rbBranch.bodyType = RigidbodyType2D.Dynamic;
    }
}
