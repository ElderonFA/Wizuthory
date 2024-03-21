using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Looting;
using UnityEditor;
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
    
    private bool canMove = true;

    private bool go;
    private bool isJump;

    private bool attack;
    private bool dubleAttack;

    public bool isDubleAttack => dubleAttack;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private Health health;
    
    private int maxHealPotion = 2;
    private int currentCountHealPotion;
    public int GetCountHealPotion => currentCountHealPotion;

    public void SetHealPotionCount(int count)
    {
        currentCountHealPotion = count;
    }

    public static Action<int> onHealPotionCountChange;
    
    private List<IItem> canTakeItems = new List<IItem>();
    
    public void SetCanMove(bool b)
    {
        anim.SetBool("isRunning", false);
        anim.SetBool("onGround", true);
        canMove = b;
    }

    public string NameCurrentAnim => anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

    public void Start()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        sr   = GetComponent<SpriteRenderer>();

        SceneController.exitToMenu += DestroySelf;
        SceneController.restartLvl += Respawn;
        SceneController.actionGetPlayerController?.Invoke(this);
        
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (!health.IsAlive)
        {
            go = false;
            return;
        }

        CheckKey();
        UpdateAttack();
    }

    public void FixedUpdate()
    {
        if (!canMove) return;

        if (health.IsAlive)
        {
            UpdateMove();
            UpdateAnimation();
        }
    }

    public void TeleportTo(Transform pos)
    {
        transform.position = pos.position;
    }
    
    private void Respawn()
    {
        health.RevivePlayer();
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
        
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (canTakeItems.Count > 0 && currentCountHealPotion < maxHealPotion)
            {
                var firstElement = canTakeItems.FirstOrDefault(x => x is HealPotion);

                firstElement?.CollectItem();

                currentCountHealPotion++;
                onHealPotionCountChange?.Invoke(currentCountHealPotion);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (currentCountHealPotion > 0)
            {
                currentCountHealPotion--;
                onHealPotionCountChange?.Invoke(currentCountHealPotion);
                health.TakeDamage(5);
            }
        }

        //Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
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
            sr.flipX = left;
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

        if (other.tag == "Item")
        {
            var currentItem = other.GetComponent<IItem>();
            
            canTakeItems.Add(currentItem);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //теги это плохо (в этой реализации (да тут вообще много плохого, сокрей бы доделать...))
        if (other.tag != "Player" 
            && other.tag != "Enemy" 
            && other.tag != "DamageAttack" 
            && other.tag != "HitBox"
            && other.tag != "WalkingLimit")
            onGround = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player" && other.tag != "Enemy"&&  other.tag != "DamageAttack" && other.tag != "HitBox")
            onGround = false;

        if (other.tag == "Item")
        {
            var item = other.GetComponent<IItem>();
            
            foreach (var itemCanTake in canTakeItems)
            {
                if (itemCanTake == item)
                {
                    canTakeItems.Remove(item);
                    break;
                }
            }
        }
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

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        SceneController.exitToMenu -= DestroySelf;
        SceneController.restartLvl -= Respawn;
    }
}
