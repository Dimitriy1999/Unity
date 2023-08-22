using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpact : MonoBehaviour
{
    public static float CalculateImpactForce(float impactVelocity, Rigidbody rb)
    {
        return 0.5f * rb.mass * impactVelocity;
    }

    public static float ForceToPounds(float forceAmount)
    {
        return forceAmount / 4.448f;
    }

    public static float CalculateImpactForce(Rigidbody rb, Collision col)
    {
        return Vector3.Dot(col.GetContact(0).normal, col.relativeVelocity) * rb.mass;
    }
}
