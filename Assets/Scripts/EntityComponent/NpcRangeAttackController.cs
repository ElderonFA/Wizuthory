using System;
using System.Collections;
using System.Collections.Generic;
using EntityComponent;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class NpcRangeAttackController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [Space]
    [SerializeField] private Transform attackSpot;
    [SerializeField] private GameObject projectilePref;
    private Projectile projectile;
    
    private PlayerController playerController;

    private float attackDelay = 3f;

    private Action onAttackReload;

    private Coroutine attackCoroutine;

    public Action<Vector3> onFindPlayer;
    public Action<Vector3> onCheckPlayerSite;
    public Action onLosePlayer;
    
    void Start()
    {
        onAttackReload += TryAttack;
    }

    public void DoRangeAttack()
    {
        if (playerController == null)
        {
            return;
        }
        
        var projectileObj = Instantiate(projectilePref, attackSpot.position, new Quaternion());
        
        projectile = projectileObj.GetComponent<Projectile>();
        
        var playerPos = playerController.transform.position;
        var difference = transform.position - playerPos;
        var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        var quatToPlayer = Quaternion.Euler(0f, 0f, rotZ + 180f);
        
        projectile.Spawn(quatToPlayer, playerPos);
        
        attackCoroutine = StartCoroutine(AttackReload());
    }

    private void TryAttack()
    {
        if (playerController == null)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            anim.SetBool("IsAttack", false);
        }
        else
        {
            anim.SetBool("IsAttack", true);
        }
    }

    private IEnumerator AttackReload()
    {
        anim.SetBool("IsAttack", false);
        
        var currentTime = 0f;

        while (currentTime < attackDelay)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        if (playerController != null)
        {
            onCheckPlayerSite?.Invoke(playerController.transform.position);
        }
        
        onAttackReload?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            onFindPlayer?.Invoke(other.transform.localPosition);
            
            playerController = other.GetComponent<PlayerController>();

            anim.SetBool("IsAttack", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            onLosePlayer?.Invoke();
            playerController = null;
            anim.SetBool("IsAttack", false);
        }
    }

    private void OnDestroy()
    {
        onAttackReload -= TryAttack;
    }
}
