using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInsertable
{
    [SerializeField] private Code accessCodes;
    private Interactable receiverData;

    public Code AccessCodes
    {
        get { return accessCodes; }
    }

    public void ClearReceiver()
    {
        receiverData.receiver = null;
    }

    public void ConnectTo(Interactable receiver)
    {
        receiverData = receiver;
        transform.SetPositionAndRotation(receiver.position, receiver.rotation);
    }

    public Interactable GetReceiverData()
    {
        return receiverData;
    }
}
