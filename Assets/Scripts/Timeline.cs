using System;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    [Header("Timeline Management")]
    [SerializeField] private Parameter parameter; // Reference to the Parameter ScriptableObject
    
    private int currentTick = 0; // Current tick of the timeline
    
    private float tickFreq; // Frequency of ticks in hertz
    
    private float tickDuration; // Game date equivalence of a tick in months
    
    private DateTime currentDate; // Current date of the timeline
    
    private float timeSinceLastTick = 0f;
    private float tickInterval => tickFreq > 0f ? 1f / tickFreq : float.MaxValue; // Time between ticks in seconds
    
    /// <summary>
    /// Event triggered at the specified tick frequency
    /// </summary>
    public Action<int> OnTick;
    
    private void Awake()
    {
        if (parameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in Timeline!");
            return;
        }
        
        tickFreq = parameter.PlayTickFreq; // default to play speed
        tickDuration = parameter.TickDuration;
        currentDate = parameter.StartDate;
    }
    
    private void Update()
    {
        if (tickFreq <= 0f) return; // Paused
        
        timeSinceLastTick += Time.deltaTime;
        
        if (timeSinceLastTick >= tickInterval)
        {
            timeSinceLastTick -= tickInterval;
            currentTick++;
            OnTick?.Invoke(currentTick);
            currentDate = currentDate.AddMonths((int)tickDuration);
            Debug.Log($"Timeline advanced to tick: {currentTick} on date: {currentDate}");
        }
    }
    
    public void SetPlaySpeed()
    {
        if (parameter != null)
            tickFreq = parameter.PlayTickFreq;
    }
    
    public void SetPauseSpeed()
    {
        tickFreq = 0f;
    }
    
    public void SetFastSpeed()
    {
        if (parameter != null)
            tickFreq = parameter.FastTickFreq;
    }
    
    public void SetVeryFastSpeed()
    {
        if (parameter != null)
            tickFreq = parameter.VeryFastTickFreq;
    }
}
