using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpotController : MonoBehaviour
{
    [SerializeField] private ImagesAnim fireAnim;
    [Space]
    [SerializeField] private List<Sprite> startAnimSprites;
    [Space] 
    [SerializeField] private SpriteMask viewMask;

    private void Start()
    {
        fireAnim.endEvent += LoopFiring;
    }

    public void StartFiring()
    {
        StartCoroutine(fireAnim.PlayOneShot(startAnimSprites));
        StartCoroutine(ShowLightMask());
    }

    private void LoopFiring()
    {
        StartCoroutine(fireAnim.PlayOneShot(fireAnim.sprites));
    }

    private IEnumerator ShowLightMask()
    {
        var currentCutoof = 1f;

        while (currentCutoof > 0.161f)
        {
            viewMask.alphaCutoff = currentCutoof;
            currentCutoof -= Time.deltaTime * 2f;
            yield return null;
        }
    }
}
