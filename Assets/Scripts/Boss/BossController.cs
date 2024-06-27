using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private float speed;
    [Space]
    [SerializeField] private SpriteMask eyeLight;

    private BossStates currentState = BossStates.Stay;
    
    private Transform playerTransform;

    private bool IsWakeuped;
    private bool go;

    private float stayTime = 1f;
    private float goTime = 1.5f;

    private float movingModifier = 2f;

    private Action<BossStates> endTimer;

    private void FixedUpdate()
    {
        if (!IsWakeuped)
        {
            return;
        }

        if (go)
        {
            if (playerTransform.position.x > transform.position.x)
            {
                sr.flipX = false;
                rb.AddForce(transform.right * speed * movingModifier);
            }
            else
            {
                sr.flipX = true;
                rb.AddForce(-transform.right * speed * movingModifier);
            }
        }
    }

    public void WakeUp()
    {
        IsWakeuped = true;
            
        anim = GetComponent<Animator>();
        anim.enabled = true;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        playerTransform = FindObjectOfType<PlayerController>().gameObject.transform;
        
        StartCoroutine(ShowEyeLight());

        endTimer += UpdateState;
    }

    private void UpdateState(BossStates newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case BossStates.Stay:
                go = false;
                StartCoroutine(StateTimer(stayTime, BossStates.Go));
                anim.SetBool("isMoving", false);
                break;
            case BossStates.Go:
                go = true;
                StartCoroutine(StateTimer(goTime, BossStates.Stay));
                anim.SetBool("isMoving", true);
                break;
            case BossStates.CastSkill:
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowEyeLight()
    {
        var cutOff = 1f;

        while (cutOff > 0.2f)
        {
            eyeLight.alphaCutoff = cutOff;
            cutOff -= Time.deltaTime / 2f;
            yield return null;
        }
        
        UpdateState(BossStates.Stay);
    }

    private IEnumerator StateTimer(float timePeriod, BossStates nextBossState)
    {
        var currentTime = 0f;

        while (currentTime < timePeriod)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        endTimer?.Invoke(nextBossState);
    }

    public void StartUpdateBoss()
    {
        UpdateState(BossStates.Stay);
    }
}

public enum BossStates
{
    Stay = 0,
    Go = 1,
    CastSkill = 2,
}
