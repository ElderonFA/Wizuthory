using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp;
    public float MaxHp => maxHp;

    private float currentHp;
    public float CurrentHp => currentHp;

    private bool isAlive;
    public bool IsAlive => isAlive;

    [SerializeField] private Animator anim;

    public delegate void OnPersonTakeDamage();
    public event OnPersonTakeDamage onPersonTakeDamage;

    public delegate void OnPersonDead();
    public event OnPersonDead onPersonDead;
    
    void Awake()
    {
        currentHp = maxHp;
        isAlive = true;
        anim.SetBool("isAlive", isAlive);
    }

    public void TakeDamage(float damage)
    {
        currentHp -= Mathf.Max(0, damage);
        onPersonTakeDamage?.Invoke();

        CheckIsAlive();

        anim.SetBool("isAlive", isAlive);

        if (isAlive)
            anim.SetBool("isTakeHit", true);
        else
            onPersonDead?.Invoke();
    }

    private void CheckIsAlive()
    {
        isAlive = currentHp > 0 ? true : false;
    }
}
