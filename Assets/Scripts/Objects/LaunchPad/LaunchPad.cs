using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [Header("Launch Pad Preview")]
    [SerializeField] private bool editLaunchDirection;
    //[ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(editLaunchDirection))]
    [SerializeField] private Color Color;
    //[ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(editLaunchDirection))]
    [SerializeField] private float lineThickness;
    [SerializeField] public KeyBind key;
    [Space(10)]
    [Header("Launch Pad Info")]
    [SerializeField] public float force;
    [SerializeField] public float angle;

    [SerializeField, HideInInspector]
    public Quaternion forceDirection;

    public Color LineColor => Color;
    public float LineThickness => lineThickness;

    public bool EditingLaunchDirection()
    {
        return editLaunchDirection;
    }

    public void DisableEditingLaunchDirection()
    {
        editLaunchDirection = false;
    }

    public void DisableEditing()
    {
        editLaunchDirection = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.TryGetComponent<Rigidbody>(out var rb)) return;

        rb.velocity = Vector3.zero;

        Quaternion launchRotation = Quaternion.Euler(-angle, forceDirection.eulerAngles.y, 0);
        Vector3 directionToLaunch = launchRotation * Vector3.forward;
        rb.AddForce(force * rb.mass * directionToLaunch, ForceMode.Impulse);
    }
}
