using System;
using System.Collections;
using System.Collections.Generic;
using Looting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SecretObject : MonoBehaviour, IObjectWithItems
{
    [SerializeField] public SecretType currentSecretType;
    [SerializeField] private int countActionForOpenSecret;
    [Space]
    [SerializeField] private AudioClip completeSound;
    [SerializeField] private AudioClip oneActionCompleteSound;
    [Space] 
    [SerializeField] private GameObject currentItem;
    [SerializeField] private int itemsCount;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private float minusEf = 2f;

    private int countCompletedActionsForSecret;

    private bool itemsWasDropped;

    public Action neededActionIsDo;

    private Coroutine currentDelayBeforeNewAction;
    private Coroutine currentDestroyCoroutine;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = oneActionCompleteSound;

        switch (currentSecretType)
        {
            case SecretType.JumpingSecret:
                neededActionIsDo += JumpSecretActionComplete;
                break;
            
            default:
                break;
        }
    }

    private void JumpSecretActionComplete()
    {
        if (currentDelayBeforeNewAction == null)
        {
            audioSource.Play();
            countCompletedActionsForSecret++;
            currentDelayBeforeNewAction = StartCoroutine(DelayBeforeNewAction());
        }

        if (countCompletedActionsForSecret == countActionForOpenSecret)
        {
            StartCoroutine(AnimDestroy());
        }
    }

    private IEnumerator DelayBeforeNewAction()
    {
        var currentTime = 0f;
        while (currentTime < 0.1f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        currentDelayBeforeNewAction = null;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DamageAttack")
        {
            currentDestroyCoroutine = StartCoroutine(AnimDestroy());
        }
    }

    public IEnumerator AnimDestroy()
    {
        if (currentDestroyCoroutine == null)
        {
            audioSource.clip = completeSound;
            audioSource.Play();

            while (spriteRenderer.color.r > 0)
            {
                var currentColor = spriteRenderer.color;

                var newColor = new Color(
                    currentColor.r - Time.deltaTime * minusEf,
                    currentColor.g - Time.deltaTime * minusEf,
                    currentColor.b - Time.deltaTime * minusEf);
                spriteRenderer.color = newColor;

                yield return null;
            }

            DropItems();
            Destroy(gameObject);
        }
    }

    public void DropItems()
    {
        if (!itemsWasDropped)
        {
            for (var i = 0; i < itemsCount; i++)
            {
                Instantiate(currentItem, transform.position, Quaternion.identity);
            }

            itemsWasDropped = true;
        }
    }
}

public enum SecretType
{
    JumpingSecret = 0,
}
