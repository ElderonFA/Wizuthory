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

    [SerializeField] private NpcAttack attackController;
    [Space]
    [SerializeField] private float speed;
    [Space]
    [SerializeField] private SpriteMask eyeLight;
    [Space] 
    [SerializeField] private List<BossSkillObj> bossSkillObjs;

    private Action castAllSkills;

    private BossStates currentState = BossStates.Stay;
    private Coroutine currentTimer;
    
    private Transform playerTransform;

    private bool IsWakeuped;
    
    private bool go;
    private bool goChanger
    {
        get => go;
        set
        {
            go = value;

            anim.SetBool("IsMoving", value);
        }
    }
    
    private bool lookLeft;
    private bool isAttack;

    private float stayTime = 1f;
    private float goTime = 1.5f;
    private float attackDelay = 2f;
    private float castDelay = 2f;

    private float movingModifier = 2f;

    private int counterForCastSkill;

    private Action<BossStates> endStateTimer;

    private void Start()
    {
        if (SceneController.playerControllerInstance.isDebug)
        {
            WakeUp();
        }
    }

    private void FixedUpdate()
    {
        if (!IsWakeuped)
        {
            return;
        }

        if (goChanger)
        {
            var currentSpeed = lookLeft ? transform.right * speed * movingModifier : -transform.right * speed * movingModifier;
            rb.AddForce(currentSpeed);
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

        endStateTimer += UpdateState;

        attackController.playerEnterToAttack += StartAttacking;
        attackController.playerExitAttack += StopAttacking;

        foreach (var bossSkillObj in bossSkillObjs)
        {
            castAllSkills += bossSkillObj.CastSkill;
        }
    }

    private void UpdateState(BossStates newState)
    {
        currentState = newState;
        counterForCastSkill++;

        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
        }

        lookLeft = playerTransform.position.x > transform.position.x;
        attackController.ChangeAttackPos(!lookLeft);

        sr.flipX = !lookLeft;

        if (counterForCastSkill == 10)
        {
            counterForCastSkill = 0;
            
            UpdateState(BossStates.CastAllSkills);
            
            return;
        }
        
        switch (newState)
        {
            case BossStates.Stay:

                if (isAttack)
                {
                    goChanger = false;
                    anim.SetBool("IsAttack", true);
                    currentTimer = StartCoroutine(StateTimer(attackDelay, BossStates.Attack));
                }
                else
                {
                    goChanger = false;
                    currentTimer = StartCoroutine(StateTimer(stayTime, BossStates.Go));
                }
                break;
            
            case BossStates.Go:
                goChanger = true;
                currentTimer = StartCoroutine(StateTimer(goTime, BossStates.Stay));
                break;
            
            case BossStates.Attack:
                anim.SetBool("IsAttack", true);
                currentTimer = StartCoroutine(StateTimer(attackDelay, BossStates.Stay));
                break;
                
            case BossStates.CastAllSkills:
                anim.SetBool("IsCastSkill", true);
                currentTimer = StartCoroutine(StateTimer(castDelay, BossStates.Stay));
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
        
        endStateTimer?.Invoke(nextBossState);
    }

    public void StartUpdateBoss()
    {
        UpdateState(BossStates.Stay);
    }
    
    private void StartAttacking()
    {
        goChanger = false;
        isAttack = true;
        
        UpdateState(BossStates.Attack);
    }

    private void StopAttacking()
    {
        isAttack = false;
        UpdateState(BossStates.Stay);
    }

    //Функции для анимации
    public void SetIsAttackAnimFalse()
    {
        anim.SetBool("IsAttack", false);
    }
    
    public void SetIsUseSkillAnimFalse()
    {
        anim.SetBool("IsCastSkill", false);
    }

    public void InvokeCastAllSkills()
    {
        castAllSkills?.Invoke();
    }
}

public enum BossStates
{
    Stay = 0,
    Go = 1,
    Attack = 2,
    CastAllSkills = 3,
}
