using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPreviewMouse : MonoBehaviour
{
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;
    [SerializeField] private Transform orientation;
    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
