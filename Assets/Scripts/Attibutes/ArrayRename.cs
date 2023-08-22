using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayRename : PropertyAttribute
{
    public Type enumType;

    public ArrayRename(Type enumType)
    {
        this.enumType = enumType;
    }
}
