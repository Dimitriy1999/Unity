using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [SerializeField] private LayerMask fadeOutLayer;

    public static int ObjectFadeOutLayer;

    private void Awake()
    {
        ObjectFadeOutLayer = GetLayerValue(fadeOutLayer);
    }


    private int GetLayerValue(LayerMask layer)
    {
        for (int i = 1; i < 32; i++)
        {
            if (1 << i == layer.value)
            {
                return i;
            }
        }
        return -1;
    }
}
