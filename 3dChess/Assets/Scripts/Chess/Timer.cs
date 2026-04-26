using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Timer : MonoBehaviour
{
    public Image I_Timer;
    public TMP_Text T_Time;

    private CanvasGroup CG;

    private DateTime startTime;
    private int duration;
    private Action onEnd;
    private bool isRunning;

    private void Awake()
    {
        CG = GetComponent<CanvasGroup>();
    }

    public void Create(int seconds, Action OnEndTimer, DateTime? realStart = null)
    {
        CG.alpha = 1;

        duration = seconds;
        onEnd = OnEndTimer;

        startTime = realStart ?? DateTime.UtcNow;
        isRunning = true;

        I_Timer.fillAmount = 1;
        T_Time.color = Color.white;
    }

    private void Update()
    {
        if (!isRunning) return;

        if (startTime > DateTime.UtcNow)
        {
            return;
        }

        double elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
        double remaining = duration - elapsed;

        if (remaining <= 0)
        {
            remaining = 0;
            isRunning = false;
            onEnd?.Invoke();
        }

        int timeInt = Mathf.CeilToInt((float)remaining);

        // Color transitions
        if (remaining < duration / 4f)
            T_Time.color = Color.red;
        else if (remaining < duration / 2f)
            T_Time.color = Color.yellow;
        else
            T_Time.color = Color.white;

        T_Time.text = timeInt + "s";

        I_Timer.fillAmount = (float)(remaining / duration);
    }

    public void Stop()
    {
        isRunning = false;
        CG.alpha = 0;
    }
}
