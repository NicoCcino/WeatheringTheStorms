using System;
using UnityEngine;

[System.Serializable]
public class Human : Singleton<Human>
{
    [SerializeField] private HumanParameter humanParameter; // Reference to the Parameter ScriptableObject
    public long HumanCount;
    private float floatValue;
    private ModifierManager modifierManager;

    /// <summary>
    /// Event triggered when the human count changes
    /// </summary>
    public Action<uint, float> OnHumanCountChanged;

    public void Start()
    {
        if (humanParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in human!");
            return;
        }
        HumanCount = (long)Mathf.Floor(humanParameter.StartValue);
        floatValue = humanParameter.StartValue;
        modifierManager = new ModifierManager();

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

    public void OnTimelineTick(uint currentTick)
    {
        // Population growth is calculated monthly
        floatValue += floatValue * (humanParameter.PopulationGrowthPerYear / 12f);
        if (floatValue <= 0) floatValue = 0;
        HumanCount = (long)Mathf.Floor(floatValue);
        if (HumanCount <= 0) Debug.Log("Human population is extinct!\n You died motherfucker!");
        float humanImpact = GetHumanImpactOnGauges();
        //Debug.Log("Human value: " + value);
        OnHumanCountChanged?.Invoke(currentTick, humanImpact);
    }

    private float GetHumanImpactOnGauges()
    {

        return HumanCount * humanParameter.PopulationGrowthPerYear;
    }

    public void AddModifier(Modifier modifier)
    {
        modifierManager.AddModifier(modifier);
        floatValue += modifier.OneShotValue;
    }
}
