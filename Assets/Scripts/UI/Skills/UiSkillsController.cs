using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Looting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiSkillsController : MonoBehaviour
{
    [SerializeField] private List<UiSkill> skillsList = new List<UiSkill>();
    [SerializeField] private Text textHelpSkill;

    private string currentControlForHelp;
    
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
                skill.OnSkillRefresh += SceneController.playerControllerInstance.SetCanDodgeTrue;
                currentControlForHelp = "Shift";
                break;
            
            case PlayerSkills.DistanceAttack:
                 skill.OnSkillRefresh += SceneController.playerControllerInstance.SetCanShootTrue;
                 currentControlForHelp = "Right mouse";
                break;
            
            case PlayerSkills.RedSparks:
                skill.OnSkillRefresh += SceneController.playerControllerInstance.SetCanRedSparksTrue;
                currentControlForHelp = "F";
                break;
            
            default:
                break;
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

        StartCoroutine(ShowHelpAboutSkill(uiSkillObject.transform));
    }

    private IEnumerator ShowHelpAboutSkill(Transform skillUiPos)
    {
        textHelpSkill.gameObject.SetActive(true);
        textHelpSkill.color = new Color(textHelpSkill.color.r, textHelpSkill.color.g, textHelpSkill.color.b, 1f);
        textHelpSkill.transform.position = new Vector3(skillUiPos.position.x, textHelpSkill.transform.position.y, 1f);

        var oldText = textHelpSkill.text;
        var newText = oldText.Replace("*", currentControlForHelp);
        textHelpSkill.text = newText;

        var showingTime = 4f;
        var currentTime = 0f;

        var colorA = 0.31f;
        var appearance = false;

        while (currentTime < showingTime)
        {
            currentTime += Time.deltaTime;

            if (appearance)
            {
                colorA += Time.deltaTime;

                if (colorA >= 1f)
                {
                    appearance = false;
                }
            }
            else
            {
                colorA -= Time.deltaTime;
                
                if (colorA <= 0.3f)
                {
                    appearance = true;
                }
            }

            textHelpSkill.color = new Color(textHelpSkill.color.r, textHelpSkill.color.g, textHelpSkill.color.b, colorA);

            yield return null;
        }

        textHelpSkill.text = "Press * to use";
        textHelpSkill.gameObject.SetActive(false);
    }

    public void UseUiSkill(PlayerSkills skillType)
    {
        StartCoroutine(skillsList.FirstOrDefault(x => x.currentSkillType == skillType).Refresh());
    }
}