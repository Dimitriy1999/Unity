using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInsertable
{
    Code AccessCodes { get; }
    void ConnectTo(Interactable reciever);
    Interactable GetReceiverData();
    void ClearReceiver();
}