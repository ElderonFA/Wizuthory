using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ladder : MonoBehaviour
{
    [SerializeField] 
    private Transform upPoint;
    public Transform GetUpPoint => upPoint;
    
    [SerializeField] 
    private Transform downPoint;
    public Transform GetDownPoint => downPoint;
    [Space] 
    [SerializeField] 
    private bool offAfterUse;

    public void UseLadder(Transform userTransform)
    {
        var upPPos = upPoint.position;
        var downPPos = downPoint.position;
        var userPos = userTransform.position;
        
        var upPointDistance = Vector2.Distance(userPos, upPPos);
        var downPointDistance = Vector2.Distance(userPos, downPPos);

        userTransform.position = upPointDistance > downPointDistance ? upPPos : downPPos;

        if (offAfterUse)
        {
            enabled = false;

            var collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;
        }
    }
}
