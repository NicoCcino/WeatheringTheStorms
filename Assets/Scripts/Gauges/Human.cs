using System;
using UnityEngine;

[System.Serializable]
public class Human : Singleton<Human>
{
    [SerializeField] private HumanParameter humanParameter; // Reference to the Parameter ScriptableObject
    public long HumanCount;

    private float floatHumanValue;
    private ModifierManager humanModifierManager;
    private ModifierManager humanImpactModifierManager;
    /// <summary>
    /// Event triggered when the human count changes
    /// </summary>
    public Action<uint, float> OnHumanCountChanged;

    [field: SerializeField] public float HumanImpact { get; private set; }
    public long PopulationDelta;

    public void Start()
    {
        if (humanParameter == null)
        {
            Debug.LogError("Parameter ScriptableObject is not assigned in human!");
            return;
        }
        HumanCount = (long)Mathf.Floor(humanParameter.StartValue);
        floatHumanValue = humanParameter.StartValue;
        humanModifierManager = new ModifierManager();
        humanModifierManager.modifierScale = humanParameter.HumanPopulationModifierScale;
        humanImpactModifierManager = new ModifierManager();
        humanImpactModifierManager.modifierScale = humanParameter.HumanPopulationImpactModifierScale;

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
        // Population growth is exponential and is calculated monthly
        float naturalGrowth = (floatHumanValue * (humanParameter.PopulationGrowthPerYear / 12f));
        float modifiedGrowth = (floatHumanValue+naturalGrowth) * humanModifierManager.ComputeModifierValue();
        float humanGrowth = naturalGrowth + modifiedGrowth;
        PopulationDelta = (long)Mathf.Floor(humanGrowth);
        floatHumanValue += humanGrowth;

        if (floatHumanValue <= 0) floatHumanValue = 0;
        HumanCount = (long)Mathf.Floor(floatHumanValue);
        if (HumanCount <= 0) Debug.Log("Human population is extinct!\n You died motherfucker!");
        HumanImpact = GetHumanImpactOnGauges();
        //Debug.Log("Human value: " + value);
        OnHumanCountChanged?.Invoke(currentTick, HumanImpact);
    }

    public float GetHumanImpactOnGauges()
    {
        float ratio = HumanCount / humanParameter.TuningValue;
        return -(float)Math.Pow(ratio, humanParameter.HumanPopulationImpactPower + (humanParameter.HumanPopulationImpactPower * humanImpactModifierManager.ComputeModifierValue())) * humanParameter.HumanPopulationImpactScale;
    }

    public void AddHumanModifier(Modifier modifier)
    {
        humanModifierManager.AddModifier(modifier);
        floatHumanValue += humanModifierManager.ComputeOneShotValue(modifier);
    }
    public void AddHumanImpactModifier(Modifier modifier)
    {
        humanImpactModifierManager.AddModifier(modifier);
        if (modifier.OneShotValue != "")
        {
            Debug.LogError("Human impact modifier don't use OneShot values!");
            return;
        }
    }
}
