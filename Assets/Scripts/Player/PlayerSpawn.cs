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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
