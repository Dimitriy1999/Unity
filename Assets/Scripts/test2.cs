using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float objectSpeedLimit;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, objectSpeedLimit);
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.up * force);
    }

    public void SetForce(float force, float objectSpeedLimit)
    {
        this.force = force;
        this.objectSpeedLimit = objectSpeedLimit;
    }
}
