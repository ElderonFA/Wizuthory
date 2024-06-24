using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCutSceneHelper : MonoBehaviour
{
    [SerializeField] private FireSpotController fireSpotFirst;
    [SerializeField] private FireSpotController fireSpotSecond;
    
    public void StartFiringSpot()
    {
        fireSpotFirst.StartFiring();
        fireSpotSecond.StartFiring();
    }
}
