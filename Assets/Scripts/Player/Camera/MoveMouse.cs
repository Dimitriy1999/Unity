using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMouse : MonoBehaviour
{
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    float xRotation;
    float yRotation;

    private Rigidbody rb;
    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (World.Instance.GetTeleportPreviewCameraState()) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }


    private void RotatePlayer()
    {
        if (World.Instance.GetTeleportPreviewCameraState()) return;

        var rotation = Quaternion.Euler(0, yRotation, 0);

        rb.MoveRotation(rotation);
    }

    private void FixedUpdate()
    {
        RotatePlayer();
    }

    public void SetMouseRotation(Vector3 rotation)
    {
        xRotation = rotation.x;
        yRotation = rotation.y;
    }

}
