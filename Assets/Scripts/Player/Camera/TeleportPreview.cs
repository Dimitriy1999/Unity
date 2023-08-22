using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPreview : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera teleportCameraPreview;
    
    public void ChangeCameraView(InputAction.CallbackContext obj)
    {
        if(obj.performed)
        {
            playerCamera.enabled = !playerCamera.enabled;
            teleportCameraPreview.enabled = !teleportCameraPreview.enabled;
        }
    }
}
