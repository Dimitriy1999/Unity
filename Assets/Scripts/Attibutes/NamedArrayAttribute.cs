using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }

    public string[] GetEnumNames()
    {
        return Enum.GetNames(typeof(ItemSaveType));
    }
}

