using System;
using System.Collections;
using System.Collections.Generic;
using EntityComponent;
using UnityEngine;

public class DistanceAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePref;
    private IProjectile projectile;
    
    private float rotationOffset = 0f;

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }

    public void DoAttack(Vector2 targetPos)
    {
        var projectileObj = Instantiate(projectilePref, transform.position, new Quaternion());
        projectile = projectileObj.GetComponent<IProjectile>();
        
        projectile.Spawn(transform.rotation, targetPos);
    }
}
