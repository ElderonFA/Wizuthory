using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//using UnityEngine.Random;

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
    [SerializeField] private NpcAttack npcAttack;

    [Header ("MinMaxDelay")]
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [Space]
    [SerializeField] private float minGoTime;
    [SerializeField] private float maxGoTime;

    private bool _canMove;
    private bool _chasePlayer;
    private bool _goLeft;
    private bool _takeOppositeLook;
    private Vector2 _actualSpeed;

    private float _waitTime;
    //public bool Stay => _waitTime > 0;
    
    private float _goTime;
    //public bool Patrol => _goTime > 0;

    void FixedUpdate()
    {
        UpdateMove();
        //Debug.Log(_chasePlayer);
    }


    private void UpdateMove()
    {
        if (!npcHealth.IsAlive || !_canMove)
            return;

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
        
        personView.flipX = _goLeft;
        npcAttack.ChangeAttackPos(_goLeft);
    }

    private void UpdateActualSpeed()
    {
        if (_chasePlayer)
        {
            if (playerTransform.position.x <= transform.position.x)
            {
                _goLeft = true;
            }
            else
            {
                _goLeft = false;
            }
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
            _actualSpeed = -transform.right * speed;
        }
        else
        {
            _actualSpeed = transform.right * speed;
        }
    }

    private float GetTimeDelay(float minTime, float maxTime) => Random.Range(minTime, maxTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _chasePlayer = true;
        }

        if (other.tag == "WalkingLimit" && !_chasePlayer)
        {
            _waitTime = GetTimeDelay(minWaitTime, maxWaitTime);
            _goTime = 0;

            _takeOppositeLook = true;
        }
        
        if (other.tag == "Death")
        {
            npcHealth.TakeDamage(npcHealth.MaxHp);
        }
        
    }

    public void SetCanMove()
    {
        _canMove = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
            _chasePlayer = false;
    }
}