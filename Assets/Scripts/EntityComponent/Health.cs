using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp;
    public float MaxHp => maxHp;

    private float currentHp;
    public float CurrentHp => currentHp;

    private bool isAlive;
    public bool IsAlive => isAlive;

    private SpriteRenderer personView;

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

        personView = transform.parent.gameObject.GetComponent<SpriteRenderer>();

        //onPersonDead += KillSelf;
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
    
    //Корутина анимации смерти и удаления персонажа со здоровьем
    private IEnumerator DespawnAnim()
    {
        var r = 1f;
        var g = 1f;
        var b = 1f;
        var a = 1f;
        while(a > 0)
        {
            yield return null;
            g -= Time.deltaTime;
            b -= Time.deltaTime;
            a -= Time.deltaTime;
            
            personView.color = new Color(r, g, b, a);
        }
        
        StopCoroutine(DespawnAnim());
        Destroy(transform.parent.gameObject);
    }
    

    public void KillSelf()
    {
        StartCoroutine(DespawnAnim());
    }
}
