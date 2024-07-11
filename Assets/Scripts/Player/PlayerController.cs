using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Looting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float flySpeed;

    [Header("Health")]
    [SerializeField] private Health health;
    
    [Header("Attack")]
    [SerializeField] private PlayerAttack attackController;
    [SerializeField] private DistanceAttack distanceAttack;
    [SerializeField] private AreaEffector2D attackEffector;
    
    [Header("Skills")]
    [SerializeField] private ImagesAnim dodgeAnim;
    [SerializeField] private GameObject redSparksSpot;
    [SerializeField] private GameObject redSpark;
    [Space]
    public bool isDebug = true;
    
    private float actualSpeed;
    public float ActualSpeed => actualSpeed;

    private bool onGround {get; set;}
    private bool left;
    public bool isLookLeft => left;
    private bool right = true;
    
    private bool canMove = true;

    private bool go;
    private bool isJump;

    private bool onLadder;
    private Ladder currentLadder;

    private bool attack;
    private bool dubleAttack;

    public bool IsAttack => attack;
    public bool IsDubleAttack => dubleAttack;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private int maxHealPotion = 2;
    private int currentCountHealPotion;
    public int GetCountHealPotion => currentCountHealPotion;
    
    private List<IItem> canTakeItems = new List<IItem>();

    private List<PlayerSkills> availableSkills = new List<PlayerSkills>();
    
    private float rightPosStaff;
    private float leftPosStaff;

    private bool canShoot;
    private bool canDodge;
    private bool canRedSparks;
    
    public static Action<int> onHealPotionCountChange;

    public void AddNewSkill(PlayerSkills newSkill)
    {
        SceneController.actionUnlockUiSkill(newSkill);
        
        availableSkills.Add(newSkill);

        switch (newSkill)
        {
            case PlayerSkills.Dodge:
                canDodge = true;
                break;
            case PlayerSkills.DistanceAttack:
                canShoot = true;
                break;
            case PlayerSkills.RedSparks:
                canRedSparks = true;
                break;
            default:
                break;
        }
    }

    public void SetHealPotionCount(int count)
    {
        currentCountHealPotion = count;
    }
    
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

        health.onPersonDead += PlayDeadAnim;

        rightPosStaff = distanceAttack.transform.localPosition.x;
        leftPosStaff = -rightPosStaff;

        dodgeAnim.endEvent += OffImmortal;
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
        anim.enabled = true;
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

            if (!attack && !dubleAttack)
            {
                distanceAttack.transform.localPosition = new Vector3(leftPosStaff, distanceAttack.transform.localPosition.y, 1);
                redSparksSpot.transform.localPosition = new Vector3(leftPosStaff, redSparksSpot.transform.localPosition.y, 1);
                
                attackController.UpdateSiteAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            right = true;
            left = false;

            go = true;

            if (!attack && !dubleAttack)
            {
                distanceAttack.transform.localPosition = new Vector3(rightPosStaff, distanceAttack.transform.localPosition.y, 1);
                redSparksSpot.transform.localPosition = new Vector3(rightPosStaff, redSparksSpot.transform.localPosition.y, 1);
                
                attackController.UpdateSiteAttack();
            }
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
                var firstElement = canTakeItems.FirstOrDefault();
                if (firstElement == null)
                {
                    return;
                }
                
                firstElement.CollectItem();

                if (firstElement is HealPotion)
                {
                    currentCountHealPotion++;
                    onHealPotionCountChange?.Invoke(currentCountHealPotion);
                }

                if (firstElement is UpScroll)
                {
                    //Здесь может быть эффект или звук
                }
            }

            if (currentLadder != null)
            {
                currentLadder.UseLadder(transform);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (health.isHaveMaxHp)
            {
                return;;
            }
            
            if (currentCountHealPotion > 0)
            {
                currentCountHealPotion--;
                onHealPotionCountChange?.Invoke(currentCountHealPotion);
                health.Heal(30);
            }
        }

        //Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            isJump = true;
            attack = false;
            dubleAttack = false;  
        }

        if (isDebug)
        {
            if (Input.GetKeyUp(KeyCode.N))
            {
                SceneController.toNewLevel?.Invoke(SceneController.currentLlv);
            }

            if (Input.GetKeyUp(KeyCode.U))
            {
                AddNewSkill(PlayerSkills.RedSparks);
            }
        }
        
        //Уворот
        if ((availableSkills.Contains(PlayerSkills.Dodge)
             && Input.GetKeyDown(KeyCode.LeftShift)
             && canDodge))
        {
            SceneController.actionUseUiSkill(PlayerSkills.Dodge);
            
            health.SetIsImmortal(true);
            DodgeProcess();

            canDodge = false;
        }

        //Дистанционная атака
        if (availableSkills.Contains(PlayerSkills.DistanceAttack) 
            && Input.GetMouseButtonDown(1)
            && canShoot)
        {
            SceneController.actionUseUiSkill(PlayerSkills.DistanceAttack);
            
            distanceAttack.DoAttack(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            canShoot = false;
        }
        
        //Сноп красных искр
        if (availableSkills.Contains(PlayerSkills.RedSparks) 
            && Input.GetKeyDown(KeyCode.F)
            && canRedSparks)
        {
            SceneController.actionUseUiSkill(PlayerSkills.RedSparks);

            var sparkCount = Random.Range(5, 10);

            for (var i = 0; i < sparkCount; i++)
            {
                Instantiate(redSpark, redSparksSpot.transform.position, Quaternion.identity);
            }
            
            canRedSparks = false;
        }
    }

    private void DodgeProcess()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
        StartCoroutine(dodgeAnim.PlayOneShot(dodgeAnim.sprites));
    }

    private void OffImmortal()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        health.SetIsImmortal(false);
    }
    
    public void SetCanDodgeTrue()
    {
        canDodge = true;
    }

    public void SetCanShootTrue()
    {
        canShoot = true;
    }

    public void SetCanRedSparksTrue()
    {
        canRedSparks = true;
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

        if (onGround && isJump)
        {
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
        
        if (other.TryGetComponent<Ladder>(out var ladder))
        {
            currentLadder = ladder;
        }

        if (other.tag == "SecretObject")
        {
            var currentSecretObject = other.GetComponent<SecretObject>();
            
            if (currentSecretObject.currentSecretType == SecretType.JumpingSecret && onGround == false && isJump)
            {
                 currentSecretObject.neededActionIsDo?.Invoke();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //теги это плохо (в этой реализации (да тут вообще много плохого, сокрей бы доделать...))
        if (other.tag != "Player"
        &&  other.tag != "Enemy"
        &&  other.tag != "DamageAttack"
        &&  other.tag != "HitBox"
        &&  other.tag != "WalkingLimit"
        &&  other.tag != "SecretObject")
        {
            isJump = false;
            onGround = true;
        }
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
        
        if (other.TryGetComponent<Ladder>(out var ladder))
        {
            currentLadder = null;
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

    public void PlayDeadAnim()
    {
        anim.SetBool("playDead", true);
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
