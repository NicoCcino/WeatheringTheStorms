using System;
using UnityEngine;

[System.Serializable]
public class Gauge
{
    [SerializeField] public GaugeParameter gaugeParameter; // Reference to the Parameter ScriptableObject
    public float value;
    public float iterationValue;

    public void Init()
    {
        if (gaugeParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in gauge!");
            return;
        }
        value = gaugeParameter.StartValue;
    }

    public void OnTick()
    {
        value += gaugeParameter.DecayingPerTick;
        if (value <= 0)
        {
            value = 0;
            Debug.Log("Gauge value is 0, you lost motherfucker!");
        }
        value = Mathf.Clamp(value, gaugeParameter.Min, gaugeParameter.Max);
    }

    public void AddModifier(Modifier modifier)
    {
        value += modifier.OneShotValue;
    }

}
