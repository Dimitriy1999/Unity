using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LookForPlayer : MonoBehaviour
{
    private const int fovDistance = 15;
    [SerializeField] private Transform player;
    [SerializeField] private bool visualizeFOV;
    [SerializeField] private int angle;
    private Transform myTransform;


    private void Awake()
    {
        myTransform = transform;
    }

    private void Update()
    {

        if (!visualizeFOV) return;

        VisualizeFov();
    }

    private void VisualizeFov()
    {
        float officalAngle = Vector3.SignedAngle(myTransform.forward, Vector3.right, Vector3.up);

        float angle = (officalAngle + this.angle / 2) * Mathf.Deg2Rad;
        float angle1 = (officalAngle - this.angle / 2) * Mathf.Deg2Rad;
        float x = CalculateXPosition(angle);
        float z = CalculateYPosition(angle);

        var x1 = CalculateXPosition(angle1);
        var z1 = CalculateYPosition(angle1);

        Vector3 newPos = new Vector3(x, myTransform.position.y, z);
        Vector3 newPos1 = new Vector3(x1, myTransform.position.y, z1);

        Debug.DrawLine(myTransform.position, newPos, Color.blue);
        Debug.DrawLine(myTransform.position, newPos1, Color.blue);
        //Debug.DrawLine(newPos, newPos1, Color.blue);
    }

    private float CalculateYPosition(float angle)
    {
        return myTransform.position.z + Mathf.Sin(angle) * fovDistance;
    }

    private float CalculateXPosition(float angle)
    {
        return myTransform.position.x + Mathf.Cos(angle) * fovDistance;
    }

    public bool SpottedPlayer(Transform enemy)
    {
        var direction = (player.position - enemy.position).normalized;

        bool inView = Vector3.Angle(enemy.forward, direction) <= angle / 2;
        bool collided = Physics.Raycast(enemy.position, direction, out RaycastHit hitInfo, fovDistance);
        if (inView && collided)
        {
            return hitInfo.transform.gameObject.layer == player.gameObject.layer;
        }
        return false;
    }

    public bool PointInView(Transform enemy)
    {
        var direction = (player.position - enemy.position).normalized;

        bool inView = Vector3.Angle(enemy.forward, direction) <= angle / 2;

        return inView;
    }
    public Vector3 GetLastPlayerPosition()
    {
        return player.position;
    }
}
