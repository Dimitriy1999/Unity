using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTowards : MonoBehaviour
{
    [SerializeField] private float forceToPushTowards;

    Rigidbody rb;
    Transform objectToPushTowards;

    private void FixedUpdate()
    {
        if (objectToPushTowards == null) return;

        var distance = Vector3.Distance(objectToPushTowards.position, transform.position);

        if (distance <= 4.5f) return;

        var direction = objectToPushTowards.position - transform.position;

        rb.AddForce(1.5f * distance * forceToPushTowards * direction);

        rb.AddForce(Vector3.down * distance);
    }

    public void Set(Rigidbody rigidbody, float forceAmount, Transform objectToPushTowards)
    {
        rb = rigidbody;
        this.objectToPushTowards = objectToPushTowards;
        forceToPushTowards = forceAmount;
    }

}
