using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct SaveData
{
    public ItemSaveType itemType;

    public Transform transform;
    public SaveData(ItemSaveType itemType, Transform transform)
    {
        this.itemType = itemType;
        this.transform = transform;
    }
}
