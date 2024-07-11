using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerController pC;
    [SerializeField] private Transform leftSiteObject;
    [SerializeField] private Transform rightSiteObject;
    [SerializeField] private Transform attackObject;

    private Vector2 leftSite;
    private Vector2 rightSite;

    public void UpdateSiteAttack()
    {
        attackObject.position = pC.isLookLeft ? leftSiteObject.position : rightSiteObject.position;
        
        if (pC.IsDubleAttack && pC.NameCurrentAnim == "attack2")
        {
            if (pC.isLookLeft)
            {
                pC.ChangeAttackAngle(-135);
            }
            else
            {
                pC.ChangeAttackAngle(-45);
            }
        }
    }
}
