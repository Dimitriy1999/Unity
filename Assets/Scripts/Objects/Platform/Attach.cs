using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour
{
    [SerializeField] private Transform player;

    Rigidbody playerRB;
    private Rigidbody rb;
    private void Awake()
    {
        playerRB = player.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //var velDif = rb.velocity - playerRB.velocity;
        playerRB.velocity = rb.velocity;
    }
}
