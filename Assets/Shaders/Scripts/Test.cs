using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayElementRename : MonoBehaviour
{
    [SerializeField] private float maxTime;
    [SerializeField] private Timer time;
    [SerializeField] private Material fadeOutMat;

    private void Awake()
    {
        time = new();
    }

    private void Update()
    {
        fadeOutMat.SetFloat("_Current_Time", (maxTime - time.Time) / maxTime);
    }
}
