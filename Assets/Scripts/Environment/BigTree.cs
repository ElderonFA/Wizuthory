using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class BigTree : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartInteract()
    {
        audioSource.Play();
        rb.simulated = true;
    }
}
