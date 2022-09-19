using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float offSetX;
    [SerializeField] private float offSetY;

    [SerializeField] private float speed;
    private Vector3 position;

    void FixedUpdate()
    {
        position = new Vector3(target.position.x + offSetX, target.position.y + offSetY, -10);
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }
}
