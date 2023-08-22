using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.XR;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Transform endTransform;
    [SerializeField] private float moveDuration;
    [Space(5)]
    [Header("Platform Preview")]
    [Space(5)]
    [SerializeField] private bool showEndPosition;
    [SerializeField] private MeshFilter mesh;

    private Vector3 initialPosition;
    private Rigidbody rb;
    private float moveSpeed;
    private bool movingUp;
    private BoolTimer moving;
    private Transform myTransform;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
        initialPosition = myTransform.position;
        moveSpeed = Vector3.Distance(endTransform.position, initialPosition) / moveDuration;
        moving = BoolTimer.Create();
    }

    private void Update()
    {
        if (!moving) return;

        var yClamp = Clamp(myTransform.position.y, initialPosition.y, endTransform.position.y);
        myTransform.position = new Vector3(myTransform.position.x, yClamp, myTransform.position.z);
    }

    private void FixedUpdate()
    {
        if (!moving) return;

        rb.MovePosition(GetDirection());
    }

    private Vector3 GetDirection()
    {
        if (movingUp) return myTransform.position + moveSpeed * Time.fixedDeltaTime * Vector3.up;

        return myTransform.position + moveSpeed * Time.fixedDeltaTime * Vector3.down;
    }

    private float Clamp(float value, float min, float max)
    {
        if (value < min && !movingUp)
        {
            value = min;
            moving.Reset();
        }
        else if (value > max && movingUp)
        {
            value = max;
            moving.Reset();
        }
        return value;
    }

    public void MoveUp()
    {
        Move(true);
    }

    public void MoveDown()
    {
        Move(false);
    }

    private void Move(bool value)
    {
        movingUp = value;
        moving.Set(moveDuration);
    }

    public void OnButtonPress()
    {
        movingUp = !movingUp;
        moving.Set(moveDuration);
    }

    private void OnDrawGizmos()
    {
        if (!showEndPosition) return;

        Gizmos.color = new Color(0, 15, 200, 0.25f);
        Gizmos.DrawMesh(mesh.sharedMesh, -1, new Vector3(transform.position.x, endTransform.position.y, transform.position.z), transform.rotation, transform.localScale);
    }
}
