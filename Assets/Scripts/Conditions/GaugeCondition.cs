using UnityEngine;

[System.Serializable]
public class GaugeCondition : ICondition
{
    [field: SerializeField] public int MinValue { get; private set; }
    [field: SerializeField] public int MaxValue { get; private set; }
    [field: SerializeField] public PromptData.PromptCategory Category { get; private set; }
    
    public GaugeCondition(int minValue, int maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public bool IsFulfilled()
    {
        if (GaugeManager.Instance == null) return false;
        
        Gauge gauge = Category switch
        {
            PromptData.PromptCategory.Climate => GaugeManager.Instance.ClimateGauge,
            PromptData.PromptCategory.Societal => GaugeManager.Instance.SocietalGauge,
            PromptData.PromptCategory.Trust => GaugeManager.Instance.TrustGauge,
            _ => null
        };
        
        if (gauge == null) return false;
        
        return gauge.value >= MinValue && gauge.value <= MaxValue;
    }
}
