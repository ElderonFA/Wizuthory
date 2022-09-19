using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class AtlasAnim : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private float frameDelay;

    private int allFramesCount;
    private int currentFrame;

    private SpriteRenderer spriteRenderer;

    private bool firstFrame = true;
    
    private void Start()
    {
        allFramesCount = atlas.spriteCount;

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        StartCoroutine(TimeSprite());
    }

    /// <summary>
    /// Анимация атласа через корутину.<br/>
    /// Нейминг спрайтов должен быть в числовом формате от 0 до конечного спрайта.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeSprite()
    {
        while (true)
        {
            if (firstFrame)
            {
                currentFrame = 0;
                firstFrame = false;
                yield return new WaitForSeconds(frameDelay);
            }

            currentFrame++;
            spriteRenderer.sprite = atlas.GetSprite(currentFrame.ToString());

            if (currentFrame == allFramesCount)
                firstFrame = true;

            yield return new WaitForSeconds(frameDelay);
        }
    }
}
