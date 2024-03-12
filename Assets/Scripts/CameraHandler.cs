using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D confiner2D;

    public static Action<PolygonCollider2D> updateCameraConfiner;

    private bool updateConfinerIsListen;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (!updateConfinerIsListen)
        {
            updateConfinerIsListen = true;
            updateCameraConfiner += UpdateCameraConfiner;
        }

        SceneController.exitToMenu += DestroySelf;
    }

    private void Start()
    {
        SceneController.actionGetCameraHandler?.Invoke(this);
    }

    private void UpdateCameraConfiner(PolygonCollider2D collider2d)
    {
        confiner2D.m_BoundingShape2D = collider2d;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneController.exitToMenu -= DestroySelf;
        
        updateCameraConfiner -= UpdateCameraConfiner;
    }
}
