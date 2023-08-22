using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Hinge Joints")]
    [SerializeField] private HingeJoint doorHingeJoint;
    [SerializeField] private float doorMinRotation = -90;
    [SerializeField] private float doorMaxRotation = 90;

    [Header("Handles")]
    [SerializeField] private Transform[] doorHandles = new Transform[2];
    [SerializeField] private float openAngle;
    [SerializeField] private bool pushHandleToOpen;

    [Header("Options")]
    [SerializeField] private bool needsKey;
    [SerializeField, Range(1, 5)] private int keysNeededToOpen = 1;
    [SerializeField] private float counterForceAmount;

    [Header("Locked / Unlocked Colors")]
    [SerializeField] private Transform doorState;
    [SerializeField] private Material lockedColor;
    [SerializeField] private Material unlockedColor;

    private float maxRotation;
    private int numberOfKeys;
    private MeshRenderer doorStateMeshRenderer;
    private Rigidbody rb;

    private Quaternion[] doorHandleOriginalRotation;
    private HingeJoint[] doorHandleHingeJoints;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        doorStateMeshRenderer = doorState.GetComponent<MeshRenderer>();
        doorHingeJoint.useLimits = true;

        if (!needsKey)
        {
            SetJointLimits(doorHingeJoint, doorMinRotation, doorMaxRotation);
            SetDoorStateColor(unlockedColor);
        }
        else
        {
            SetJointLimits(doorHingeJoint, 0, 0);
            SetDoorStateColor(lockedColor);
        }

        maxRotation = transform.eulerAngles.y + doorMaxRotation;
        SetupDoorHandles();
    }

    private void SetupDoorHandles()
    {
        doorHandleOriginalRotation = new Quaternion[doorHandles.Length];
        doorHandleHingeJoints = new HingeJoint[doorHandles.Length];

        for (int i = 0; i < doorHandles.Length; i++)
        {
            doorHandleOriginalRotation[i] = doorHandles[i].rotation;
            doorHandleHingeJoints[i] = doorHandles[i].GetComponent<HingeJoint>();
            SetJointLimits(doorHandleHingeJoints[i], -openAngle, openAngle);
        }
    }

    private void SetJointLimits(HingeJoint hingeJoint, float min, float max)
    {
        JointLimits jointLimits = hingeJoint.limits;
        jointLimits.min = min;
        jointLimits.max = max;
        hingeJoint.limits = jointLimits;
    }

    private void FixedUpdate()
    {
        Vector3 counterForce = -rb.velocity;
        rb.AddForce(counterForce * counterForceAmount);
    }

    private void Update()
    {
        if (numberOfKeys < keysNeededToOpen || !pushHandleToOpen) return;

        for (int i = 0; i < doorHandles.Length; i++)
        {
            float angle = Quaternion.Angle(doorHandles[i].rotation, doorHandleOriginalRotation[i]);

            if (angle < openAngle) continue;

            SetJointLimits(doorHingeJoint, doorMinRotation, doorMaxRotation);
            break;
        }
    }

    public void TryToUnlockDoor()
    {
        numberOfKeys++;

        if (numberOfKeys < keysNeededToOpen) return;

        SetDoorStateColor(unlockedColor);
        needsKey = false;

        if (pushHandleToOpen) return;

        SetJointLimits(doorHingeJoint, doorMinRotation, doorMaxRotation);
    }

    public void LockDoor()
    {
        numberOfKeys--;

        if (numberOfKeys >= keysNeededToOpen) return;

        float rotationDifference = maxRotation - transform.eulerAngles.y;
        float newLimits = Mathf.Clamp(doorMaxRotation - rotationDifference, doorMinRotation, doorMaxRotation);

        SetJointLimits(doorHingeJoint, newLimits - 0.001f, newLimits);

        SetDoorStateColor(lockedColor);
        needsKey = true;
    }

    private void SetDoorStateColor(Material mat)
    {
        doorStateMeshRenderer.material = mat;
    }
}
