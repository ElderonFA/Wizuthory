using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private Transform[] layers;
    [SerializeField] private float[] coeff;

    private int LayersCount;
    void Start()
    {
        LayersCount = layers.Length;
    }

    void Update()
    {
        for(int i = 0; i < LayersCount; i++)
        {
            layers[i].position = new Vector2(transform.position.x * coeff[i], layers[i].position.y);
        }        
    }
}
