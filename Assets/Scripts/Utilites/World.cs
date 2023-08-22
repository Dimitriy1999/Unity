using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class World : MonoBehaviour
{
    [Header("Physics Refresh Rate")]
    [SerializeField] private float physicsHz = 60;
    [Space(20f)]

    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera teleportCameraPreview;
    [SerializeField] private Transform brokenObjectHolder;

    public static World Instance { get; private set; }

    public Teleport Teleport { get; private set; }

    public GameObject Player { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]


    private static void Init()
    {
        Instance = null;
    }

    public void Awake()
    {
        Instance = this;
        Time.fixedDeltaTime = 1 / physicsHz;
        Teleport = player.GetComponent<Teleport>();
        Player = player;
        Application.targetFrameRate = 400;
    }

    public void TeleportPreview(InputAction.CallbackContext obj)
    {
        if (obj.performed && Teleport.TeleportActive())
        {
            playerCamera.enabled = !playerCamera.enabled;
            teleportCameraPreview.enabled = !teleportCameraPreview.enabled;
        }
    }

    public void RevertCameraStatesToDefault()
    {
        playerCamera.enabled = true;
        teleportCameraPreview.enabled = false;
    }

    public bool GetPlayerCameraState()
    {
        return playerCamera.enabled;
    }

    public bool GetTeleportPreviewCameraState()
    {
        return teleportCameraPreview.enabled;
    }

    public void SetTeleportPreviewPosition(Vector3 position)
    {
        teleportCameraPreview.transform.position = new Vector3(position.x, position.y, position.z);
    }

    public void ChangePlayerViewAfterTeleportPreview()
    {
        if (GetPlayerCameraState()) return;

        playerCamera.transform.GetComponent<MoveMouse>().SetMouseRotation(teleportCameraPreview.transform.eulerAngles);
    }

    public Transform BrokenObjectHolder()
    {
        return brokenObjectHolder;
    }
}
