using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{
    private float remainingSeconds = 0f;
    private float totalSeconds;

    private float playBackSpeed = 1f;

    bool playing = true;
    bool reverse = false;

    public bool isPlaying
    {
        get { return playing; }
    }

    // This is a non monobehavior function. 
    // This to decrease the number of components a game object will have without coupling all the code together.
    // To make a timer it's basically var myTimer = new Timer(howmuchTimeYouwant);
    // When a parameter is already declared with a value, that parameter becomes optional.
    public Timer(float duration, bool playOnStart = true)
    {
        totalSeconds = duration;
        playing = playOnStart;
    }

    // Tick function that needs to be run on a monobehavior update loop.
    // Lets the timer increment and reach to it's duration. 
    // Normal pass would be time.deltaTime but we could pass in our custom rates as well.
    // Could be pretty much used as a counter as well.
    public void Tick(float rate)
    {
        if (!playing) return;

        remainingSeconds += rate * playBackSpeed;

        if (remainingSeconds >= totalSeconds)
        {
            OnTimerEnd?.Invoke();
        }
    }

    public void SetPlayBackSpeed(float speed)
    {
        playBackSpeed = speed;
    }

    public void PlayFromStart()
    {
        remainingSeconds = 0f;
        playing = true;
    }

    public void SetDuration(float newDuration)
    {
        remainingSeconds = newDuration;
    }

    public void Stop()
    {
        playing = false;
    }

    public float GetProgress()
    {
        return remainingSeconds / totalSeconds;
    }

    public event Action OnTimerEnd;
}
