using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct SaveDTO
{
    public int version;
    public string modified;
    [JsonProperty("saved_items")]
    public List<DTO> savedItems;

    public SaveDTO(int version, int maxCapacity, string modifiedTime)
    {
        this.version = version;
        savedItems = new(maxCapacity);
        modified = modifiedTime;
    }
}
