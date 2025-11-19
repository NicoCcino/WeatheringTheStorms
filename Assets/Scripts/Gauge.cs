using System;
using UnityEngine;

[System.Serializable]
public class Gauge
{
    [SerializeField] private GaugeParameter gaugeParameter; // Reference to the Parameter ScriptableObject
    public int value;
    private float Speed;
    //private ModifierManager modifierManager;

    public void Init()
    {
        if (gaugeParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in gauge!");
            return;
        }
        value = gaugeParameter.StartValue;
        Speed = gaugeParameter.StartSpeed;
    }

    public void OnTimelineTick(int currentTick)
    {
        value += (int)(Speed);
        if (value < gaugeParameter.Min) value = gaugeParameter.Min;
        if (value > gaugeParameter.Max) value = gaugeParameter.Max;
        //Debug.Log("Gauge value: " + value);
        // compute modifier effects
    }

    // public void ApplyModifier(Modifier modifier)
    // {
    //     value += (int)(modifier.Value);
    //     if (value < min) value = min;
    //     if (value > max) value = max;
    // }
}
