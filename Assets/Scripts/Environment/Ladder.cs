using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] 
    private Transform upPoint;
    public Transform GetUpPoint => upPoint;
    
    [SerializeField] 
    private Transform downPoint;
    public Transform GetDownPoint => downPoint;

    public void UseLadder(Transform userTransform)
    {
        var upPPos = upPoint.position;
        var downPPos = downPoint.position;
        var userPos = userTransform.position;
        
        var upPointDistance = Vector2.Distance(userPos, upPPos);
        var downPointDistance = Vector2.Distance(userPos, downPPos);

        userTransform.position = upPointDistance > downPointDistance ? upPPos : downPPos;
    }
}
