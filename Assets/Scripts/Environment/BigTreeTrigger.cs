using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTreeTrigger : MonoBehaviour
{
    private int punchCount = 0;
    private AudioSource audioSource;
    
    [SerializeField] 
    private int needPunchForStartInteract = 5;
    [SerializeField] 
    private BigTree bigTree;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DamageAttack")
        {
            audioSource.Play();
            punchCount++;

            if (punchCount == needPunchForStartInteract)
            {
                bigTree.StartInteract();
                
                Destroy(gameObject, audioSource.clip.length);
            }
        }
    }
}
