using System;
using UnityEngine;

[System.Serializable]
public class ComputePower : Singleton<ComputePower>
{
    [SerializeField] private ComputePowerParameter computePowerParameter; // Reference to the Parameter ScriptableObject
    public float value;
    private ModifierManager modifierManager;

    public void Start()
    {
        if (computePowerParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in ComputePower!");
            return;
        }
        value = computePowerParameter.StartValue;
        modifierManager = new ModifierManager();
        modifierManager.Init(computePowerParameter.BaseModifier);

        // Subscribe to the OnTick event from Timeline
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnTimelineTick;
        }
    }

    private void OnDisable()
    {
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick -= OnTimelineTick;
        }
    }

    public void OnTimelineTick(int currentTick)
    {
        value += modifierManager.ComputeModifierValue();
        if (value <= 0) value = 0;
        //Debug.Log("Human value: " + value);
    }

    public void SpendComputePower(int amount)
    {
        if (amount > value) { Debug.LogError("Not enough compute power to spend!"); return; }
        value -= amount;
    }
}
