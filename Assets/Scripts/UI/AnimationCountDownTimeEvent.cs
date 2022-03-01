using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCountDownTimeEvent : AnimationManager
{
    [SerializeField] GameObject countDownTimer;
    public void StartCountingTime()
    {
        countDownTimer.GetComponent<CountDownTime>().StartCountDownTimer();
    }
    public void StopCountingTime()
    {
        countDownTimer.GetComponent<CountDownTime>().ResetTime();
    }
}
