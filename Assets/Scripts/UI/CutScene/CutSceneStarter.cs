using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CutSceneStarter : MonoBehaviour
{
    [SerializeField] private CutSceneConfig cutSceneConfig;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            CutSceneController.OnStartCutScene.Invoke(cutSceneConfig.GetConfigCutScene);
            Destroy(gameObject);
        }
    }
}
