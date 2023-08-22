using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float time;
    public float Time
    {
        get
        {
            return UnityEngine.Time.time - time;
        }
    }

    public Timer()
    {
        Reset();
    }

    public Timer(float duration)
    {
        time = UnityEngine.Time.time - duration;
    }

    public void SetTimer(float duration)
    {
        time = UnityEngine.Time.time - duration;
    }

    public void Reset()
    {
        time = UnityEngine.Time.time;
    }

    public bool CheckElapsedTime(float durationSeconds, bool afterDuration)
    {
        float elapsed = UnityEngine.Time.time - time;
        if (afterDuration)
        {
            if (elapsed >= durationSeconds)
            {
                return true;
            }
        }
        else
        {
            if (elapsed <= durationSeconds)
            {
                return true;
            }
        }
        return false;
    }
}
