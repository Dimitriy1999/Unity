using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;


public enum TeleportState
{ 
    None,
    Active,
    NotActive
}


public class Teleport : MonoBehaviour
{
    [Header("Line Visuals")]
    [SerializeField] private float arcAngle = 25;
    [SerializeField] private GameObject showLine;
    [SerializeField] private float lineAmount;
    [SerializeField] private float timeStep;
    [SerializeField] private float gravity;
    [SerializeField] private int maxStrength = 15;
    [SerializeField] private Transform teleportSpot;
    [SerializeField] private Material[] teleportStateMaterials = new Material[2]; 

    [Header("Others")]
    [SerializeField] private LayerMask layerToIgnore;
    [SerializeField] private Camera firstCamera;

    private float radius;
    private bool teleportActive;
    private BoolTimer playerTeleporting;
    private static List<GameObject> list = new List<GameObject>();
    private Timer teleportTimer;
    private TeleportState teleportState = TeleportState.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]


    private static void Init()
    {
        list = new();
    }

    private void Awake()
    {
        for (int i = 0; i < lineAmount; i++)
        {
            if (list.Count < lineAmount)
            {
                var test = Instantiate(showLine, showLine.transform.parent);
                test.SetActive(false);
                list.Add(test);
            }
        }
        playerTeleporting = BoolTimer.Create();
        radius = teleportSpot.localScale.y / 2;
        teleportTimer = new(0.5f);
        SetTeleportState(TeleportState.Active);
    }

    private void Update()
    {
        ChangeColorIfCanTeleport();


        if (PlayerTeleporting() || !teleportActive) return;

        TeleportVisual();
    }

    private bool PlayerTeleporting()
    {
        if (playerTeleporting)
        {
            if (Vector3.Distance(transform.position, teleportSpot.position) <= 1.05f)
            {
                playerTeleporting.Reset();
                teleportTimer.Reset();
            }
            transform.position = new Vector3(teleportSpot.position.x,
            teleportSpot.position.y + 1, teleportSpot.position.z);
            return true;
        }
        return false;
    }
    private void ChangeColorIfCanTeleport()
    {
        if(teleportTimer.Time < 0.5f)
        {
            if (teleportState == TeleportState.NotActive) return;

            SetTeleportState(TeleportState.NotActive);

            for(int i = 0; i < list.Count; i++)
            {
                var pointInLine = list[i];
                pointInLine.GetComponent<MeshRenderer>().material = teleportStateMaterials[0];
            }
            teleportSpot.GetComponent<MeshRenderer>().material = teleportStateMaterials[0];
        }
        else
        {
            if (teleportState == TeleportState.Active) return;

            SetTeleportState(TeleportState.Active);

            for (int i = 0; i < list.Count; i++)
            {
                var pointInLine = list[i];
                pointInLine.GetComponent<MeshRenderer>().material = teleportStateMaterials[1];
            }
            teleportSpot.GetComponent<MeshRenderer>().material = teleportStateMaterials[1];
        }
    }

    private void TeleportVisual()
    {
        int index = 0;
        Vector3 startVelocity = (firstCamera.transform.forward * arcAngle);
        for (float i = 0; i < list.Count; i += timeStep)
        {
            if (index >= list.Count) break;

            var point = transform.position + i * startVelocity;
            point.y = transform.position.y + startVelocity.y * i + (gravity / 2f * i * i);
            list[index].transform.position = point;
            index++;
        }
        FindTeleportSpot();
        World.Instance.SetTeleportPreviewPosition(new Vector3(teleportSpot.position.x, teleportSpot.position.y + 1.5f, teleportSpot.position.z));
    }

    public void TeleportKeyPressed(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            if (teleportActive && World.Instance.GetTeleportPreviewCameraState())
            {
                UpdateTeleportState();
                DisableTeleportVisual();
            }
            else
            {
                teleportActive = true;
            }
        }
        else if (obj.canceled && teleportActive && World.Instance.GetPlayerCameraState())
        {
            UpdateTeleportState();
            DisableTeleportVisual();
        }
    }

    private void DisableTeleportVisual()
    {
        teleportActive = false;
        ShowVisibleArc(list.Count - 1, false);
        World.Instance.RevertCameraStatesToDefault();
    }

    private void UpdateTeleportState()
    {
        if (!playerTeleporting && teleportTimer.Time >= 0.5f)
        {
            World.Instance.ChangePlayerViewAfterTeleportPreview();
            playerTeleporting.Set(1f);
        }
    }

    public void CanceledTeleport(InputAction.CallbackContext obj)
    {
        if (obj.performed && teleportActive)
        {
            DisableTeleportVisual();
            teleportSpot.position = Vector3.zero;
        }
    }

    public void ChangeArcAngleOnScroll(InputAction.CallbackContext obj)
    {
        var readValue = obj.ReadValue<Vector2>().normalized;

        arcAngle = Mathf.Clamp(arcAngle += readValue.y, 5, maxStrength);
    }

    private void FindTeleportSpot()
    {
        RaycastHit finalPoint = new();
        finalPoint.point = Vector3.zero;

        for (int i = 0; i < list.Count - 1; i++)
        {
            var currentPointInArc = list[i].transform;
            var nextPointInArc = list[i + 1].transform;
            var distance = Vector3.Distance(nextPointInArc.position, currentPointInArc.position);
            var direction = (nextPointInArc.position - currentPointInArc.position);

            bool collision = Physics.Raycast(currentPointInArc.position, direction, distance);

            if (collision)
            {
                ShowVisibleArc(i, true);
                Physics.Raycast(currentPointInArc.position, Vector3.down, out RaycastHit raycastDown, 100);
                teleportSpot.position = raycastDown.point;
                return;
            }

        }
        ShowVisibleArc(GetAvailableSpot(), true);
    }

    private int GetAvailableSpot()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var pointInArc = list[i].transform;

            var collided = Physics.Raycast(pointInArc.position, Vector3.down, out RaycastHit raycast, 100);
            if (collided)
            {
                teleportSpot.position = raycast.point;
                return i;
            }
            pointInArc.gameObject.SetActive(false);
        }
        return -1;
    }

    private void ShowVisibleArc(int lastIndex, bool visible)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i <= lastIndex)
            {
                list[i].SetActive(visible);
            }
            else
            {
                list[i].SetActive(!visible);
            }
        }
    }

    private void SetTeleportState(TeleportState state)
    {
        teleportState = state;
    }

    public bool TeleportActive()
    {
        return teleportActive;
    }

}
