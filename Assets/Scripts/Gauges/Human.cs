using System;
using UnityEngine;

[System.Serializable]
public class Human : Singleton<Human>
{
    [SerializeField] private HumanParameter humanParameter; // Reference to the Parameter ScriptableObject
    public float value;
    private ModifierManager modifierManager;

    public void Start()
    {
        if (humanParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in human!");
            return;
        }
        value = humanParameter.StartValue;
        modifierManager = new ModifierManager();
        modifierManager.Init(humanParameter.BaseModifier);

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
        if (value <= 0) Debug.Log("Human population is extinct!\n You died motherfucker!");
        //Debug.Log("Human value: " + value);
    }

    public void AddModifier(Modifier modifier)
    {
        modifierManager.AddModifier(modifier);
    }
}
