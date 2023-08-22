using System.Collections.Generic;
using UnityEngine;

public class DrawCenterOfMass : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rb.transform.TransformPoint(rb.centerOfMass), 0.05f);
    }
}
