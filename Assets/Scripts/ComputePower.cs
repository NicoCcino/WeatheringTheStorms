using System;
using UnityEngine;

[System.Serializable]
public class ComputePower : Singleton<ComputePower>
{
    [SerializeField] private ComputePowerParameter computePowerParameter; // Reference to the Parameter ScriptableObject
    public int value;
    private float floatValue;
    private ModifierManager modifierManager;
    
    // Event triggered when computePower value changes
    public event Action<int> OnCP;

    private void UpdateValue()
    {
        int newValue = Mathf.FloorToInt(floatValue);
        if (newValue != value)
        {
            value = newValue;
            OnCP?.Invoke(value);
        }
    }

    public void Start()
    {
        if (computePowerParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in ComputePower!");
            return;
        }
        floatValue = computePowerParameter.StartValue;
        value = computePowerParameter.StartValue;
        modifierManager = new ModifierManager();
        modifierManager.Init(computePowerParameter.BaseModifier);
        OnCP?.Invoke(value);

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
        floatValue += modifierManager.ComputeModifierValue();
        if (floatValue <= 0) floatValue = 0;
        UpdateValue();
        //Debug.Log("Human value: " + value);
    }

    public void SpendComputePower(int amount)
    {
        if (amount > value) { Debug.LogError("Not enough compute power to spend!"); return; }
        floatValue -= amount;
        UpdateValue();
        //Debug.Log("Spent " + amount + " compute power. Current value: " + value);
    }

    public void AddComputePower(int amount)
    {
        floatValue += amount;
        UpdateValue();
        //Debug.Log("Added " + amount + " compute power. Current value: " + value);
    }
}
