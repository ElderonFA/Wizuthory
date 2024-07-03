using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class FireWormController : MonoBehaviour
{
    [SerializeField] private NpcRangeAttackController npcRangeAttackController;
    [SerializeField] private Health health;
    [Space] 
    [SerializeField] private float speed;

    private Animator anim;
    private Rigidbody2D rb;

    private Coroutine currentActionCoroutine;

    private float movingTime = 2f;
    private float stayTime = 3f;

    private bool isLookLeft;
    public bool IsLookLeft
    {
        get => isLookLeft;
        set
        {
            if (value != isLookLeft)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1f);
            }

            isLookLeft = value;
        }
    }
    
    private bool isMoving;
    private bool IsMoving
    {
        get => isMoving;
        set
        {
            isMoving = value;
            anim.SetBool("isMoving", value);
        }
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        health.onPersonDead += PlayDeadAnim;
        health.onPersonTakeDamage += PlayTakeHitAnim;
        
        npcRangeAttackController.onFindPlayer += StopMoving;
        npcRangeAttackController.onLosePlayer += StartStay;
        npcRangeAttackController.onCheckPlayerSite += UpdateLookWithPlayerPos;
        
        StartMoving();
    }

    private void UpdateLookWithPlayerPos(Vector3 playerPos)
    {
        IsLookLeft = playerPos.x < transform.localPosition.x;
    }

    private IEnumerator Moving()
    {
        var currentTimerValue = 0f;
        
        while (currentTimerValue < movingTime)
        {
            currentTimerValue += Time.deltaTime;
            
            yield return null;
        }

        IsLookLeft = !IsLookLeft;
        StartStay();
    }

    private IEnumerator Stay()
    {
        var currentTimerValue = 0f;
        
        while (currentTimerValue < stayTime)
        {
            currentTimerValue += Time.deltaTime;
            
            yield return null;
        }
        
        StartMoving();
    }

    private void StartMoving()
    {
        IsMoving = true;
        currentActionCoroutine = StartCoroutine(Moving());
    }

    private void StartStay()
    {
        IsMoving = false;
        currentActionCoroutine = StartCoroutine(Stay());
    }

    private void StopMoving(Vector3 playerPos)
    {
        IsLookLeft = playerPos.x < transform.localPosition.x;
        
        if (currentActionCoroutine != null)
        {
            IsMoving = false;
            StopCoroutine(currentActionCoroutine);
        }
    }

    //Функции для анимации
    public void MoveForAnim()
    {
        var currentMoveVector = IsLookLeft ? -transform.right : transform.right;
        rb.AddForce(currentMoveVector * speed);
    }
    
    private void PlayDeadAnim()
    {
        health.enabled = false;
        anim.SetBool("playDead", true);
    }
    
    private void PlayTakeHitAnim()
    {
        anim.SetBool("isTakeHit", true);
    }

    public void SetFalseForDeadAnim()
    {
        anim.SetBool("playDead", false);
    }

    public void DestroySelfAfterDead()
    {
        anim.enabled = false;
        health.KillSelf();
    }

    public void SetFalseForTakeHit()
    {
        anim.SetBool("isTakeHit", false);
    }
    
    public void DoAttack()
    {
        npcRangeAttackController.DoRangeAttack();
    }

    private void OnDestroy()
    {
        npcRangeAttackController.onFindPlayer -= StopMoving;
        npcRangeAttackController.onLosePlayer -= StartStay;
    }
}
