using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Looting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSkillsController : MonoBehaviour
{
    [SerializeField] private List<UiSkill> skillsList = new List<UiSkill>();

    private void Start()
    {
        SceneController.actionGetUiSkillsController?.Invoke(this);
    }

    public void UnlockUiSkill(PlayerSkills skillType)
    {
        var skill = skillsList.FirstOrDefault(x => x.currentSkillType == skillType);

        switch (skillType)
        {
            case PlayerSkills.Dodge:
                break;
            case PlayerSkills.DistanceAttack:
                 skill.OnSkillRefresh += SceneController.playerControllerInstance.SetCanShootTrue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null);
        }

        StartCoroutine(ShowSkill(skill.gameObject));
    }

    private IEnumerator ShowSkill(GameObject uiSkillObject)
    {
        uiSkillObject.transform.localScale =  Vector3.zero;
        
        uiSkillObject.gameObject.SetActive(true);

        var newValueScale = 0f;
        while (newValueScale < 1f)
        {
            newValueScale += Time.deltaTime;
            uiSkillObject.transform.localScale = new Vector3(newValueScale, newValueScale, 1f);
            yield return null;
        }
    }

    public void UseUiSkill(PlayerSkills skillType)
    {
        StartCoroutine(skillsList.FirstOrDefault(x => x.currentSkillType == skillType).Refresh());
    }
}