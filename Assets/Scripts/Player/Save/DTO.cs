using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Mathematics;

[Serializable]
public struct DTO
{
    [JsonProperty("item_type")]
    public ItemSaveType itemType;

    [JsonProperty("item_id")]
    public int itemId;

    [JsonProperty("item_position")]
    public SerializableVector3 position;

    [JsonProperty("item_rotation")]
    public SerializableQuaternion rotation;

    public DTO(ItemSaveType itemType, float3 position, Quaternion rotation, int itemId)
    {
        this.itemType = itemType;
        this.position = position;
        this.rotation = rotation;
        this.itemId = itemId;
    }
   
}
