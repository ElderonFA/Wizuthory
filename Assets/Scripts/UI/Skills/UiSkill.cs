using System;
using System.Collections;
using Looting;
using UnityEngine;
using UnityEngine.UI;

public class UiSkill: MonoBehaviour
{
    [SerializeField] public  PlayerSkills currentSkillType;
    [SerializeField] private float delay;
    [SerializeField] private Image refreshImage;

    public Action OnSkillRefresh;
    
    public IEnumerator Refresh()
    {
        refreshImage.fillAmount = 1;
        
        var currentTime = 0f;
        while (currentTime < delay)
        {
            currentTime += Time.deltaTime;
            refreshImage.fillAmount = 1 - currentTime / delay;
            yield return null;
        }
        
        OnSkillRefresh?.Invoke();
    }
}
