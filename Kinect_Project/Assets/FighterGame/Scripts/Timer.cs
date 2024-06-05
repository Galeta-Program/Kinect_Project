using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float startTime;
    float duration;

    public Timer(float _duration = -1)
    {
        startTime = Time.time;
        duration = _duration;
    }

    public void Start()
    {
        startTime = Time.time;
    }

    public void Start(float _duration)
    {
        duration = _duration;
        startTime = Time.time;
    }

    public void SetDuration(float _duration)
    {
        duration = _duration;
    }

    public bool isTimeOut()
    {
        return Time.time - startTime > duration;
    }
}
