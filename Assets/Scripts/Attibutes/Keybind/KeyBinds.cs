using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinds : PropertyAttribute
{
    public KeyBind keyBinds;
    public KeyBinds(KeyBind keybind)
    {
        keyBinds = keybind;
    }
}
