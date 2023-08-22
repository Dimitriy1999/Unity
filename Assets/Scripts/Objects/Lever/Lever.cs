using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    [SerializeField] private float pullAngle;
    [Space(15)]
    [Header("Events")]
    [SerializeField] private UnityEvent OnPullUp;
    [SerializeField] private UnityEvent OnPullDown;

    private Transform myTransform;
    private Vector3 originalEulerAngles;

    private void Awake()
    {
        myTransform = transform;
        originalEulerAngles = myTransform.eulerAngles;
    }

    private void Update()
    {
        bool pullingDown = IsPullingDown();
        bool pullingUp = IsPullingUp();

        if (PullPassedAngle(pullingDown))
        {
            OnPullDown.Invoke();
        }
        else if (PullPassedAngle(pullingUp))
        {
            OnPullUp.Invoke();
        }
    }

    private bool IsPullingDown()
    {
        Vector3 eulerAngles = myTransform.eulerAngles;
        return eulerAngles.x < 180 && eulerAngles.y < 180 && eulerAngles.z < 180;
    }

    private bool IsPullingUp()
    {
        Vector3 eulerAngles = myTransform.eulerAngles;
        float xAngle = eulerAngles.x;
        float yAngle = eulerAngles.y;
        float zAngle = eulerAngles.z;
        return (xAngle >= 180 && xAngle <= 360 - pullAngle - 10) ||
            (yAngle >= 180 && yAngle <= 360 - pullAngle - 10) ||
            (zAngle >= 180 && zAngle <= 360 - pullAngle - 10);
    }

    private bool PullPassedAngle(bool pulling)
    {
        Vector3 eulerAngles = myTransform.eulerAngles;
        float xAngle = Mathf.Abs(Mathf.DeltaAngle(originalEulerAngles.x, eulerAngles.x));
        float yAngle = Mathf.Abs(Mathf.DeltaAngle(originalEulerAngles.y, eulerAngles.y));
        float zAngle = Mathf.Abs(Mathf.DeltaAngle(originalEulerAngles.z, eulerAngles.z));
        return pulling && (xAngle >= pullAngle || yAngle >= pullAngle || zAngle >= pullAngle);
    }

}
