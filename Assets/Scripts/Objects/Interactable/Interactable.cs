using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Interactable
{
    public Vector3 position;
    public Quaternion rotation;
    public Transform receiver;

    public Interactable(Vector3 position, Quaternion rotation, Transform recieverObject)
    {
        this.position = position;
        this.rotation = rotation;
        receiver = recieverObject;
    }
}
