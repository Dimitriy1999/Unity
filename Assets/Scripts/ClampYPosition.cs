using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampYPosition : MonoBehaviour
{
    private Vector3 startPos;
    Vector3 endPos;
    Rigidbody rb;
    private void Awake()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        endPos = new Vector3(transform.position.x, startPos.y - 0.5f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > startPos.y)
        {
            transform.position = startPos;
        }
    }
}
