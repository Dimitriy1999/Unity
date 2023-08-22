using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjectInRadiusGravity : MonoBehaviour
{
    [SerializeField] private float upForce;
    [SerializeField] private float forceSpeedLimit;
    [SerializeField] private float forceToPushTowards;
    [Space(5)]
    [Header("Information (DONT EDIT)")]
    [SerializeField] private float totalForceInPounds;
    [SerializeField] private int counter;

    private void OnTriggerEnter(Collider other)
    {
        var collidedObject = other.gameObject;
        
        if (!collidedObject.TryGetComponent<Rigidbody>(out var rb)) return;

        rb.useGravity = false;
        if (!collidedObject.TryGetComponent<test2>(out _))
        {
            test2 forceUp = collidedObject.AddComponent<test2>();
            var force = rb.mass * upForce;
            totalForceInPounds += force / 4.448f;
            counter += 1;
            forceUp.SetForce(force, forceSpeedLimit);
        }

        if(!collidedObject.TryGetComponent<ForceTowards>(out _))
        {
           var forceTowards = collidedObject.AddComponent<ForceTowards>();
            forceTowards.Set(rb, forceToPushTowards, transform);
        }
    }
}
