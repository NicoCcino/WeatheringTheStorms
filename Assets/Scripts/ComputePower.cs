using System;
using UnityEngine;

[System.Serializable]
public class ComputePower : Singleton<ComputePower>
{
    [SerializeField] public ComputePowerParameter computePowerParameter; // Reference to the Parameter ScriptableObject
    public int value;
    private float floatValue;

    // Event triggered when computePower value changes
    public event Action<int> OnCP;
    public Action<ComputePowerLootData> OnComputePowerLootSpawn;

    private void UpdateValue()
    {
        int newValue = Mathf.FloorToInt(floatValue);
        if (newValue != value)
        {
            value = newValue;
            OnCP?.Invoke(value);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        floatValue = computePowerParameter.StartValue;
        value = computePowerParameter.StartValue;
        OnCP?.Invoke(value);

    }
    public void Start()
    {
        if (computePowerParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in ComputePower!");
            return;
        }
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

    uint lastSpawnedTick;
    public void OnTimelineTick(uint currentTick)
    {
        floatValue += computePowerParameter.BaseModifier;
        if (floatValue <= 0) floatValue = 0;
        UpdateValue();


        uint ticksSinceLastSpawn = currentTick - lastSpawnedTick;
        if (ticksSinceLastSpawn > 1 / computePowerParameter.SpawnFrequency)
        {
            OnComputePowerLootSpawn?.Invoke(new ComputePowerLootData(computePowerParameter.ComputePowerClickableGain));
            lastSpawnedTick = currentTick;
        }
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
