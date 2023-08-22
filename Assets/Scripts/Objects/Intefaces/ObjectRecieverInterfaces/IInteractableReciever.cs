using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Code
{
    CODE_A = 1,
    CODE_B = 2,
    CODE_C = 4,
    CODE_D = 8,
    CODE_E = 0x10,
    CODE_F = 0x20,
    CODE_G = 0x40,
    CODE_H = 0x80,
    CODE_I = 0x100,
    CODE_J = 0x200,
    CODE_K = 0x400
}

public enum State
{
    EMPTY = 0,
    SWITCHING = 1,
    EJECTING = 2,
    HOVERING = 3,
    INSERTED = 4,
    SLIDING = 5
}

public interface IInteractableReciever
{
    void OnReceiverEnter(Transform recievedObject);
    void SetState(State state);
    State GetState();
}
