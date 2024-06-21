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
        skill.gameObject.SetActive(true);

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
    }

    public void UseUiSkill(PlayerSkills skillType)
    {
        StartCoroutine(skillsList.FirstOrDefault(x => x.currentSkillType == skillType).Refresh());
    }
}