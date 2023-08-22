using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Button : MonoBehaviour
{
    [SerializeField] private bool hasParent;

#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasParent))]
#endif
    [SerializeField] private Transform parentObject;
    [SerializeField] private bool goingToMove;
    [SerializeField] private float pushBackForce = 1f;
    [Space(10)]
    [Header("Button Tower")]
    [SerializeField] private Transform buttonTower;
    [Header("Button Start Point")]
    [SerializeField] private Transform startPoint;
    [Header("Button End Point")]
    [SerializeField] private Transform endPoint;


    [Header("Unity Events")]
    [Space(5)]
    [SerializeField] private UnityEvent onPress;

    bool buttonPressed;
    float distance;
    Rigidbody rb;
    Transform myTransform;
    bool outOfRange;
    private void Awake()
    {
        myTransform = transform;
        rb = GetComponent<Rigidbody>();
        distance = Vector3.Distance(startPoint.position, endPoint.position);
    }

    private void Update()
    {
        outOfRange = Vector3.Distance(myTransform.position, endPoint.position) >= distance;

        if(goingToMove)
        {
            myTransform.position = new Vector3(myTransform.position.x, endPoint.position.y, myTransform.position.z);
        }

        if (!outOfRange) return;

        myTransform.position = startPoint.position;
        buttonPressed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (buttonPressed || (collision.gameObject != buttonTower.gameObject && collision.gameObject.transform != parentObject)) return;

        onPress.Invoke();
        buttonPressed = true;
    }

    private void FixedUpdate()
    {
        if (!outOfRange)
        {
            var direction = CalculateDirection();

            rb.AddForce(rb.mass * pushBackForce * direction, ForceMode.Force);
        }

    }

    private Vector3 CalculateDirection()
    {
        return startPoint.position - endPoint.position;
    }
}
