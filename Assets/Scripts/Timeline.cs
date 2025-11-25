using System;
using UnityEngine;

public class Timeline : Singleton<Timeline>
{
    [Header("Timeline Management")]
    [SerializeField] private TimeLineParameter timeLineParameter; // Reference to the Parameter ScriptableObject

    public uint CurrentTick = 0; // Current tick of the timeline

    public float tickFreq; // Frequency of ticks in hertz

    public float tickDuration; // Game date equivalence of a tick in months

    public DateTime currentDate; // Current date of the timeline

    private float timeSinceLastTick = 0f;
    public float tickInterval => tickFreq > 0f ? 1f / tickFreq : float.MaxValue; // Time between ticks in seconds

    public TimeLineParameter TimeLineParameter { get => timeLineParameter; }

    /// <summary>
    /// Event triggered at the specified tick frequency
    /// </summary>
    public Action<uint> OnTick;

    protected void Start()
    {
        if (timeLineParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in Timeline!");
            return;
        }

        tickFreq = timeLineParameter.PlayTickFreq; // default to play speed
        tickDuration = timeLineParameter.TickDuration;
        currentDate = timeLineParameter.StartDate;
    }

    private void Update()
    {
        if (tickFreq <= 0f) return; // Paused

        timeSinceLastTick += Time.deltaTime;

        if (timeSinceLastTick >= tickInterval)
        {
            timeSinceLastTick -= tickInterval;
            CurrentTick++;
            OnTick?.Invoke(CurrentTick);
            currentDate = currentDate.AddMonths((int)tickDuration);
            // Debug.Log($"Timeline advanced to tick: {currentTick} on date: {currentDate}");
        }

        if (CurrentTick >= timeLineParameter.YearsWinCondition * 12)
        {
            //TODO : Manage win properly
            Debug.Log("Win win win");
            SetPauseSpeed();
        }
    }

    public void SetPlaySpeed()
    {
        if (timeLineParameter != null)
            tickFreq = timeLineParameter.PlayTickFreq;
    }

    public void SetPauseSpeed()
    {
        tickFreq = 0f;
    }

    public void SetFastSpeed()
    {
        if (timeLineParameter != null)
            tickFreq = timeLineParameter.FastTickFreq;
    }

    public void SetVeryFastSpeed()
    {
        if (timeLineParameter != null)
            tickFreq = timeLineParameter.VeryFastTickFreq;
    }
}
