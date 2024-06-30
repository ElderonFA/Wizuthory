using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(ImagesAnim), typeof(SpriteRenderer))]
public class BossSkillObj : MonoBehaviour
{
    private ImagesAnim castSkillAnim;
    private BoxCollider2D damageCollider;
    private SpriteRenderer sr;
    
    private Vector2 castDelayRange = new Vector2(0f, 1f);
    
    void Start()
    {
        castSkillAnim = GetComponent<ImagesAnim>();
        damageCollider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        damageCollider.enabled = false;
        castSkillAnim.endEvent += AnimEnd;
    }

    public void CastSkill()
    {
        StartCoroutine(CastSkillAfterDelay());
    }

    private IEnumerator CastSkillAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(castDelayRange.x, castDelayRange.y));
        
        sr.enabled = true;
        StartCoroutine(castSkillAnim.PlayOneShot(castSkillAnim.sprites));
    }

    private void AnimEnd()
    {
        damageCollider.enabled = true;
        
        sr.enabled = false;

        StartCoroutine(DisableDamageColliderAfterDelay());
    }

    private IEnumerator DisableDamageColliderAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        damageCollider.enabled = false;
    }
}
