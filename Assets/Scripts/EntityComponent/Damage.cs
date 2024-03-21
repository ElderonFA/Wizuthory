using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private float damageCount;
    [SerializeField] private bool deleteAfterGiveDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HitBox"))
        {
            collision.GetComponent<Health>().TakeDamage(damageCount);

            if (deleteAfterGiveDamage)
            {
                Destroy(gameObject);
            }
        }
    }
}
