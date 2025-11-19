using System;
using UnityEngine;

[System.Serializable]
public class Gauge
{
    [SerializeField] private GaugeParameter gaugeParameter; // Reference to the Parameter ScriptableObject
    public int value;
    private ModifierManager modifierManager;

    public void Init()
    {
        if (gaugeParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in gauge!");
            return;
        }
        value = gaugeParameter.StartValue;
        modifierManager = new ModifierManager();
        modifierManager.Init(gaugeParameter.BaseModifier);
    }

    public void OnTimelineTick(int currentTick)
    {
        value += modifierManager.ComputeModifierValue();
        if (value < gaugeParameter.Min) value = gaugeParameter.Min;
        if (value > gaugeParameter.Max) value = gaugeParameter.Max;
        //Debug.Log("Gauge value: " + value);
    }

    public void AddModifier(Modifier modifier)
    {
        modifierManager.AddModifier(modifier);
    }
}
