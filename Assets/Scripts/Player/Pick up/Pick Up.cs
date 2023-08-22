using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    [SerializeField] private Transform PickUpSpot;
    [SerializeField] private new Transform camera;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float range;
    [SerializeField] private float pickUpSpeed;

    private Transform objectWePickedUp;
    private Rigidbody pickedUpRigidbody;

    public void PickUpItem(InputAction.CallbackContext obj)
    {
        if (!obj.performed) return;

        var validItem = Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, range, interactableLayer);

        if (!validItem || objectWePickedUp != null || Vector3.Distance(transform.position, hitInfo.transform.position) >= range)
        {
            ResetData();
            return;
        }


        objectWePickedUp = hitInfo.transform;
        if (!objectWePickedUp.TryGetComponent<Rigidbody>(out pickedUpRigidbody))
        {
            ResetData();
            return;
        }

        pickedUpRigidbody.freezeRotation = true;


        if (!objectWePickedUp.TryGetComponent<IInsertable>(out var insertable)) return;

        var recieverData = insertable.GetReceiverData();

        if (recieverData.receiver == null) return;

        var reciever = recieverData.receiver.GetComponent<IInteractableReciever>();

        if (reciever.GetState() != State.INSERTED) return;

        reciever.SetState(State.EMPTY);
        pickedUpRigidbody.isKinematic = false;
        insertable.ClearReceiver();
    }

    private void Update()
    {
        if (objectWePickedUp == null) return;

        if(!objectWePickedUp.gameObject.activeInHierarchy)
        {
            ResetData();
            return;
        }

        objectWePickedUp.forward = PickUpSpot.forward;

        if (Vector3.Distance(transform.position, objectWePickedUp.position) >= range)
        {
            ResetData();
        }
    }

    private void FixedUpdate()
    {
        if (objectWePickedUp == null || pickedUpRigidbody == null) return;

        var direction = PickUpSpot.position - objectWePickedUp.position;

        pickedUpRigidbody.velocity = direction.magnitude * pickUpSpeed * Time.fixedDeltaTime * direction;
    }
                                   
    public void ResetData()
    {
        objectWePickedUp = null;

        if (pickedUpRigidbody == null) return;

        pickedUpRigidbody.freezeRotation = false;
        pickedUpRigidbody = null;
    }
}
