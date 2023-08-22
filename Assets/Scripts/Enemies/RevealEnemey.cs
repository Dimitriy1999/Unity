using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RevealEnemey : MonoBehaviour
{
    [SerializeField] private int glowOutlineWidthSpeed = 2;
    [SerializeField] private int min = 0;
    [SerializeField] private int max = 5;
    private Outline glowOutline;

    bool invert;
    BoolTimer glowOn;

    private void Start()
    {
        glowOutline = GetComponent<Outline>();
        glowOn.Set(5);
    }

    private void Update()
    {
        glowOutline.OutlineWidth = Mathf.Clamp(glowOutline.OutlineWidth, min, max);

        if (!glowOn && glowOutline.OutlineWidth <= 0) return;

        if (glowOn)
        {
            glowOutline.OutlineWidth += glowOutlineWidthSpeed * Time.deltaTime;
        }
        else
        {
            glowOutline.OutlineWidth -= glowOutlineWidthSpeed * Time.deltaTime;
        }
    }

    public void ResetTimer()
    {
        if (glowOn || glowOutline.OutlineWidth > 0) return;

        glowOn.Reset();
        glowOn.Set(5);
    }

    public void Set(int speed, int minWidth, int maxWidth)
    {
        glowOutlineWidthSpeed = speed;
        min = minWidth;
        max = maxWidth;
    }

}
