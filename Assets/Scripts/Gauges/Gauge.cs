using System;
using UnityEngine;

[System.Serializable]
public class Gauge
{
    [SerializeField] public GaugeParameter gaugeParameter; // Reference to the Parameter ScriptableObject
    public float value;
    public float iterationValue;
    public Action<float> OnGaugeModified;
    public ModifierManager modifierManager;

    public void Init()
    {
        if (gaugeParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in gauge!");
            return;
        }
        value = gaugeParameter.StartValue;
        modifierManager = new ModifierManager();
        modifierManager.modifierScale = gaugeParameter.ModifierScale;
    }


    public void OnHumanCountChanged(float humanImpact)
    {
        iterationValue = humanImpact + modifierManager.ComputeModifierValue();
        value += iterationValue;
        OnGaugeModified?.Invoke(value);
        if (value <= 0) value = 0;
        if (value < gaugeParameter.Min) value = gaugeParameter.Min;
        if (value > gaugeParameter.Max) value = gaugeParameter.Max;
        //Debug.Log("Gauge value: " + value);
    }

    public void AddModifier(Modifier modifier)
    {
        modifierManager.AddModifier(modifier);
        value += modifier.OneShotValue;
        if (modifier.OneShotValue != 0)
            OnGaugeModified?.Invoke(value);
    }

}
