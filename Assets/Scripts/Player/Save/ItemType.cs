using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    [SerializeField] private ItemSaveType itemSaveType;


    private void Awake()
    {
        if (itemSaveType != ItemSaveType.None) return;

        Debug.LogError($"Object \"{name}\" has script \"ItemType\" and is set to \"None\". Please change it");
    }

    public ItemSaveType EnumType()
    {
        return itemSaveType;
    }
}
