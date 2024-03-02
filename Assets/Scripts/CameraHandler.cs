using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D confiner2D;

    public static Action<PolygonCollider2D> updateCameraConfiner;

    private static bool updateConfinerIsListen;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (!updateConfinerIsListen)
        {
            updateConfinerIsListen = true;
            updateCameraConfiner += UpdateCameraConfiner;
        }
    }

    private void UpdateCameraConfiner(PolygonCollider2D collider2d)
    {
        confiner2D.m_BoundingShape2D = collider2d;
    }

    private void OnDestroy()
    {
        updateCameraConfiner -= UpdateCameraConfiner;
    }
}
