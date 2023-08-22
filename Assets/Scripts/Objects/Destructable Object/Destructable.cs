using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Transform brokenObject;
    [SerializeField] private float minForce;
    [SerializeField] private int coefficent;

    private int originalHealth;
    private Transform objectHolder;
    private Rigidbody rb;
    private Transform myTransform;
    private float collisionStartTime;
    private void Awake()
    {
        myTransform = transform;
        originalHealth = health;
        rb = GetComponent<Rigidbody>();
        objectHolder = World.Instance.BrokenObjectHolder();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collidedForce;
        if (!collision.transform.TryGetComponent<Rigidbody>(out var collidedRigidbody))
        {
            collidedForce = ObjectImpact.ForceToPounds(ObjectImpact.CalculateImpactForce(rb, collision));
        }
        else
        {
            float impactForce = ObjectImpact.CalculateImpactForce(collidedRigidbody, collision);
            collidedForce = ObjectImpact.ForceToPounds(impactForce);

            float massRatio = collidedRigidbody.mass / rb.mass;
            collidedForce *= massRatio;
        }

        if (collidedForce < minForce) return;

        health -= (int)collidedForce;

        if (health > 0) return;

        var objectThatIsBroken = Instantiate(brokenObject, myTransform.position, myTransform.rotation, objectHolder);
        objectThatIsBroken.GetComponent<ExplosionForce>().SetImpactSpeed(rb.velocity, myTransform);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        myTransform.gameObject.SetActive(false);
    }

    public void ResetObjectData()
    {
        health = originalHealth;
    }
}
