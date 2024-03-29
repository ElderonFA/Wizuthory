using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float flySpeed;

    [SerializeField] private AreaEffector2D attackEffector;

    private float actualSpeed;
    public float ActualSpeed => actualSpeed;

    private bool onGround {get; set;}
    private bool left;
    public bool isLookLeft => left;
    private bool right = true;
    private bool go;
    private bool isJump;

    private bool attack;
    private bool dubleAttack;

    public bool isDubleAttack => dubleAttack;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private Health health;

    public string NameCurrentAnim => anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        sr   = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (health.IsAlive)
        {
            CheckKey();
            UpdateAttack();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        if (health.IsAlive)
            UpdateMove();

        UpdateAnimation();
    }

    private void CheckKey()
    {
        //Движение влево, вправо
        if (Input.GetKeyDown(KeyCode.A))
        {
            left = true;
            right = false;

            go = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            right = true;
            left = false;

            go = true;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            if (!right)
            {
                go = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            if (!left)
            {
                go = false;
            }
        }


        //Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && onGround == true)
        {
            isJump = true;
            attack = false;
            dubleAttack = false;  
        }
    }

    private void UpdateMove()
    {      
        //Актуальная скорость
        actualSpeed = onGround ? speed : flySpeed;
        
        if (attack || dubleAttack) 
        {
            //Не двигаемся, если бьем
            actualSpeed = 0f;   
        }
        else
        {
            //В какую сторону смотреть
            sr.flipX = left ? true : false;
        }

        if (go)
        {
            if (left)
            {
                rb.AddForce(-transform.right * actualSpeed);
            }

            if (right)
            {
                rb.AddForce(transform.right * actualSpeed);
            }
        }

        if (isJump)
        {
            isJump = false;
            rb.AddForce(transform.up * jumpPower);
        }                    
    } 

    private void UpdateAttack()
    {
        if (!onGround)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
            {
                dubleAttack = true;
            }
            else
            {
                attack = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed secondary button.");
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isRunning", go);

        anim.SetBool("isJump", isJump); 

        anim.SetBool("onGround", onGround);

        anim.SetBool("isAttack", attack);
        anim.SetBool("isDubbleAttack", dubleAttack);
    }

    public void ChangeAttackAngle(float angle)
    {
        attackEffector.forceAngle = angle;
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Death")
        {
            health.TakeDamage(health.MaxHp);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != "Player")
            onGround = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
            onGround = false;
    }


    //Обнуление анимационных переменных
    private void nullIsJump()
    {
        anim.SetBool("isJump", false);
    }  

    private void nullIsAttack()
    {
        attack = false;
        anim.SetBool("isAttack", false);
    }  

    private void nullIsDubbleAttack()
    {
        dubleAttack = false;
        anim.SetBool("isDubbleAttack", false);
    } 

    public void NullAnimation(string name)
    {
        anim.SetBool(name, false);
    }
}
