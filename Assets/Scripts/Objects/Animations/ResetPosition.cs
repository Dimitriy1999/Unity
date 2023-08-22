using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] private float duration;
    BoolTimer timer;
    Vector3 startingPosition;
    void Awake()
    {
        timer = BoolTimer.Create();
        timer.Set(duration);
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer) return;

        transform.position = new Vector3(startingPosition.x, startingPosition.y + 2f, startingPosition.z);
        timer.Set(duration);
    }
}
