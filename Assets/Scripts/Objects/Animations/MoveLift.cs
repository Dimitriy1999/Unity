using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLift : MonoBehaviour
{
    [SerializeField] private float smoothAmount;
    [SerializeField] private float forceAmount;
    [SerializeField] private float massOffset;
    [SerializeField] private float downSpeedLimit;
    [SerializeField] private float downSpeedCounterForce;
    [SerializeField] private Transform player;
    [SerializeField] private Transform endPoint;

    private bool movingUp;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private float mass;
    private float forceToPushLift;
    private Vector3 startPoint;
    private Vector3 pointToGo;
    private bool playerOnPlatform;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if(rb == null)
        {
            Debug.LogError($"Rigidbody component not found on the object {gameObject}");
        }
        mass = rb.mass * 10 + massOffset;
        startPoint = transform.position;
        pointToGo = endPoint.position;
    }

    private void Update()
    {
        var distance = pointToGo == endPoint.position ? pointToGo.y - transform.position.y : transform.position.y - pointToGo.y;

        if (distance < 0)
        {
            rb.isKinematic = true;
            transform.position = new Vector3(transform.position.x, pointToGo.y, transform.position.z);
            return;
        }

        var gravityCounterAct = movingUp ? 1 : 10;
        float updatedForceValue = forceAmount - smoothAmount / Mathf.Clamp(distance, 0.25f, 1000);
        forceToPushLift = Mathf.Clamp(updatedForceValue / gravityCounterAct, mass, float.MaxValue);
    }

    private void FixedUpdate()
    {
        if (transform.position.y == pointToGo.y) return;

        rb.AddForce(moveDirection * forceToPushLift);

        //Added in for now, not sure if its really "physics" 
        if(pointToGo == startPoint)
        {
            float speedDecrease = rb.velocity.magnitude > downSpeedLimit ? downSpeedCounterForce : 1;

            rb.AddForce(Vector3.up * forceToPushLift * speedDecrease);
        }
    }

    public void OnButtonPress()
    {
        movingUp = !movingUp;
        rb.isKinematic = false;

        moveDirection = movingUp ? Vector3.up : Vector3.down;

        if(moveDirection == Vector3.up)
        {
            pointToGo = endPoint.position;
        }
        else
        {
            pointToGo = startPoint;
        }

    }

    public bool OnPlatform()
    {
        return playerOnPlatform;
    }

    public bool MovingDown()
    {
        return !movingUp;
    }

    public Vector3 Velocity()
    {
        return rb.velocity;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != player.gameObject.layer) return;

        playerOnPlatform = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != player.gameObject.layer) return;

        playerOnPlatform = false;
    }
}