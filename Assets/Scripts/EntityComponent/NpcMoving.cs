using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NpcMoving : MonoBehaviour
{
    [Header ("Parameters")]
    [SerializeField] private float speed;

    [Header ("Components")]
    [SerializeField] private Health npcHealth;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer personView;
    [SerializeField] public NpcAttack npcAttack;

    [Header ("MinMaxDelay")]
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [Space]
    [SerializeField] private float minGoTime;
    [SerializeField] private float maxGoTime;

    private bool _canMove;
    private bool _chasePlayer;
    public bool _isAttack;
    public bool _goLeft;
    private bool _takeOppositeLook;
    private Vector2 _actualSpeed;

    private float _waitTime;
    
    private float _goTime;

    private void Start()
    {
        npcAttack.playerEnterToAttack += SetIsAttackTrue;
        npcAttack.playerExitAttack += SetIsAttackFalse;
    }

    private void AfterCollideWalkingLimit()
    {
        _waitTime = GetTimeDelay(minWaitTime, maxWaitTime);
        _goTime = 0;

        _takeOppositeLook = true;
        _chasePlayer = false;
    }

    private void SetIsAttackTrue()
    {
        UpdateLook();
        _isAttack = true;
    }
    
    private void SetIsAttackFalse()
    {
        _isAttack = false;
    }

    void FixedUpdate()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (!npcHealth.IsAlive || !_canMove)
            return;

        if (_isAttack)
        {
            return;
        }

        if (_chasePlayer)
        {
            animator.SetBool("IsMove", true);
            UpdateActualSpeed();
            rigidBody.AddForce(_actualSpeed);
        }
        else
        {
            if (_waitTime > 0)
            {
                animator.SetBool("IsMove", false);
                _waitTime -= Time.deltaTime;
                return;
            }

            if (_goTime > 0)
            {
                animator.SetBool("IsMove", true);
                rigidBody.AddForce(_actualSpeed);

                _goTime -= Time.deltaTime;
                
                if (_goTime <= 0)
                    _waitTime = GetTimeDelay(minWaitTime, maxWaitTime);
            }
            else
            {
                UpdateActualSpeed();
                _goTime = GetTimeDelay(minGoTime, maxGoTime);
            }
        } 
        
        UpdateLook();
    }

    public void UpdateLook()
    {
        personView.flipX = !_goLeft;
        npcAttack.ChangeAttackPos(!_goLeft);
    }

    private void UpdateActualSpeed()
    {
        if (_chasePlayer)
        {
            _goLeft = playerTransform.position.x > transform.position.x;
        }
        else
        {
            if (_takeOppositeLook)
            {
                _goLeft = !_goLeft;
                _takeOppositeLook = false;
            }
            else
            {
                _goLeft = Random.Range(0f, 100f) >= 50f ? false : true;
            }
        }
        
        if (_goLeft)
        {
            _actualSpeed = transform.right * speed;
        }
        else
        {
            _actualSpeed = -transform.right * speed;
        }
    }

    private float GetTimeDelay(float minTime, float maxTime) => Random.Range(minTime, maxTime);
    
    public void SetCanMove()
    {
        _canMove = true;
    }

    public void SetPlayerTransform(Transform transform)
    {
        playerTransform = transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _chasePlayer = true;
        }
        
        if (other.tag == "Death")
        {
            npcHealth.TakeDamage(npcHealth.MaxHp);
        }

        if (other.tag == "WalkingLimit")
        {
            AfterCollideWalkingLimit();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            _chasePlayer = false;
        }
    }
}