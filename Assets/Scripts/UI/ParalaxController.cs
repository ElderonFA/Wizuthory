using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private Transform[] layers;
    [SerializeField] private float[] coeff;

    private Transform playerTransform;

    private int LayersCount;
    private void Start()
    {
        GetPlayerTransform();
        LayersCount = layers.Length;
    }

    private void Update()
    {
        for(int i = 0; i < LayersCount; i++)
        {
            layers[i].position = new Vector2(playerTransform.position.x * coeff[i], layers[i].position.y);
        }        
    }

    private void GetPlayerTransform()
    {
        playerTransform = GameObject.FindWithTag("CMCamera").transform;
    }
}
