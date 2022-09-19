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

    void FixedUpdate()
    {
        leftSite = new Vector2(leftSiteObject.position.x, leftSiteObject.position.y);
        rightSite = new Vector2 (rightSiteObject.position.x, rightSiteObject.position.y);

        attackObject.position = pC.isLookLeft ? leftSite : rightSite;
        
        if (pC.isDubleAttack && pC.NameCurrentAnim == "attack2")
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

    private void UpdateSiteAttack()
    {
        if (pC.ActualSpeed == 0f)
            return;

        leftSite = new Vector2(leftSiteObject.position.x, leftSiteObject.position.y);
        rightSite = new Vector2 (rightSiteObject.position.x, rightSiteObject.position.y);

        attackObject.position = pC.isLookLeft ? leftSite : rightSite;
    }
}
