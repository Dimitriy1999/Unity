using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float Y_OFFSET;
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + Y_OFFSET, target.position.z);
    }
}
