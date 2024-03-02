using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CameraConfinerUpdate : MonoBehaviour
{
    void Start()
    {
        CameraHandler.updateCameraConfiner?.Invoke(GetComponent<PolygonCollider2D>());
    }
}
