using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private PlayerController playerController;
    
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.TeleportTo(transform);

        SceneController.restartLvl += RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        playerController.TeleportTo(transform);
    }

    private void OnDestroy()
    {
        SceneController.restartLvl -= RespawnPlayer;
    }
}
