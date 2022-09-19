using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class ImagesAnim : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float frameDelay;

    private int allFramesCount;
    private int currentFrame = -1;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        allFramesCount = sprites.Count;

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        StartCoroutine(TimeSprite());
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
}
