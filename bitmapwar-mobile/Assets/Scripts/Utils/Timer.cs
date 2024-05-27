using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool IsCountDown = false;

    private Coroutine durationCor;
    private Coroutine CountDownCort;

    private bool IsStart = false;

    private int countdownTime;

    public static Timer inst;

    private long NowServerTime;

    private long serverDeltaTime;

    private long startGameTime = 0;

    private int StartTime;
    private int NextRoundTime;

    private void Awake()
    {
        inst = this;
    }

    public void StartCountDown()
    {
        IsCountDown = true;
    }

    public void SetEndTime(int serverEndTime)
    {
        Debug.Log(">>>>>> Set End Time:" + serverEndTime);
        StartTime = serverEndTime + (int)serverDeltaTime;
    }

    public void SetNextStartTime(int nextRoundTime)
    {
        Debug.Log(">>>>>> Set Next Time:" + nextRoundTime);
        NextRoundTime = nextRoundTime + (int)serverDeltaTime;
    }

    public void StartGame()
    {
        IsStart = true;
        durationCor = StartCoroutine(CountTime());
        if (CountDownCort != null)
        {
            MainPage.inst.UpdateNextRoundTimer(0);
        }
    }

    public void EndGame(int nextRound)
    {
        MainPage.inst.mySoldiers.Clear();
        MainPage.inst.UpdateTurn(0);
        SetNextStartTime(nextRound);

        CountDownCort = StartCoroutine(CountDownTimer());

        IsStart = false;
    }
    
    public void SetServerTime(long time)
    {
        var clientTime = GetClientNowTime();
        NowServerTime = time;
        serverDeltaTime = NowServerTime - clientTime;
        Debug.Log("Client Time: " + clientTime + " Server Time: " + NowServerTime);
    }

    public long GetClientNowTime()
    {
        DateTime now = DateTime.Now;

        // 转换为UTC时间
        DateTime utcNow = now.ToUniversalTime();

        // 将时间转换为时间戳（以秒为单位）
        long timestamp = (long)(utcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        return timestamp;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(1.0f);
        var nowTime = GetClientNowTime();
        var dt = NextRoundTime - (int)nowTime;
        
        MainPage.inst.UpdateNextRoundTimer(dt);
        if(dt > 0)
        {
            CountDownCort = StartCoroutine(CountDownTimer());
        }
    }
    
    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(1.0f);
        var clientNowTime = GetClientNowTime() + serverDeltaTime;
        var dt = StartTime - clientNowTime ;
        //Debug.Log("Dt: " + dt);

        MainPage.inst.SetDurationTime(dt);
        if(dt > 0)
        {
            durationCor = StartCoroutine(CountTime());
        }
    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
