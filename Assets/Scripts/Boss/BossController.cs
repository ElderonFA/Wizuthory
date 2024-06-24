using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    private Animator anim;
    
    [SerializeField] private SpriteMask eyeLight;

    public void WakeUp()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.enabled = true;
        StartCoroutine(ShowEyeLight());
    }

    private IEnumerator ShowEyeLight()
    {
        var cutOff = 1f;

        while (cutOff > 0.2f)
        {
            eyeLight.alphaCutoff = cutOff;
            cutOff -= Time.deltaTime / 2f;
            yield return null;
        }
    }
}
