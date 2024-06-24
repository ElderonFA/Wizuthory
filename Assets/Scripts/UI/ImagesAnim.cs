using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class ImagesAnim : MonoBehaviour
{
    [SerializeField] public List<Sprite> sprites;
    [SerializeField] private float frameDelay;
    [Space] 
    [SerializeField] private bool isOneShot;

    public Action endEvent;

    private int allFramesCount;
    private int currentFrame = -1;

    private SpriteRenderer spriteRenderer;

    private Coroutine currentCoroutine;

    private void Start()
    {
        allFramesCount = sprites.Count;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!isOneShot)
        {
            currentCoroutine = StartCoroutine(TimeSprite());
        }
    }

    /// <summary>
    /// Анимация изображений через корутину.<br/>
    /// Выставлять в порядке от первого к последнему.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeSprite()
    {
        while (true)
        {
            currentFrame++;
            spriteRenderer.sprite = sprites[currentFrame];

            if (currentFrame == allFramesCount - 1)
                currentFrame = 0;

            yield return new WaitForSeconds(frameDelay);
        }
    }

    public IEnumerator PlayOneShot(List<Sprite> listSpritesAnim)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        
        currentFrame = 0;
        
        while (currentFrame <= listSpritesAnim.Count - 1)
        {
            spriteRenderer.sprite = listSpritesAnim[currentFrame];
            currentFrame++;
            
            yield return new WaitForSeconds(frameDelay);
        }
        
        endEvent?.Invoke();
    }
}
