using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class KeyReciever : MonoBehaviour, IInteractableReciever
{
    [SerializeField] private Code accessCode;
    [SerializeField] private float keyInsertedZOffset;
    [Space(10)]
    [Header("Key Preview")]
    [SerializeField] private Mesh keyMesh;
    [SerializeField] Quaternion rotation;
    [Space(20)]
    [Header("Events")]
    [SerializeField] private UnityEvent OnReceived;                
    [SerializeField] private UnityEvent OnRemoved;

    private PickUp playerInteract;
    private BoolTimer receiveCooldown;
    private State state;

    private void Awake()
    {
        receiveCooldown = new BoolTimer();
        playerInteract = World.Instance.Player.GetComponent<PickUp>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (receiveCooldown) return;

        OnReceiverEnter(collision.transform);
    }

    private void OnDrawGizmos()
    {
        if (state == State.INSERTED) return;

        keyMesh.RecalculateNormals();
        var position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + (-transform.forward * keyInsertedZOffset);
        var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - rotation.eulerAngles.y, transform.rotation.z + rotation.eulerAngles.z);
        Gizmos.color = new Color(0, 0.35f, 1f, 0.75f);
        Gizmos.DrawMesh(keyMesh, -1, position, newRotation, new Vector3(1, 1, 1));
    }

    public void OnReceiverEnter(Transform insertableTransform)
    {
        if (!insertableTransform.TryGetComponent(out IInsertable insertableObject)) return;

        if (state == State.INSERTED || insertableObject.AccessCodes != accessCode) return;

        var rigidbody = insertableTransform.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        insertableObject.ConnectTo(CreateReceiverData());

        OnReceived.Invoke();

        state = State.INSERTED;

        playerInteract.ResetData();
    }

    private Interactable CreateReceiverData()
    {
        var position = new Vector3(transform.position.x, transform.position.y, transform.position.z - keyInsertedZOffset);
        var newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - rotation.eulerAngles.y, transform.rotation.z + rotation.eulerAngles.z);

        return new Interactable(position, newRotation, transform);
    }

    public void SetState(State state)
    {
        OnRemoved.Invoke();
        receiveCooldown.Set(0.25f);
        this.state = state;
    }

    public State GetState()
    {
        return state;
    }
}
