using UnityEngine;

[System.Serializable]
public class GaugeCondition : ICondition
{
    [field: SerializeField] public int MinValue { get; private set; }
    [field: SerializeField] public int MaxValue { get; private set; }
    [field: SerializeField] public EventData.EventCategory Category { get; private set; }
    
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
            EventData.EventCategory.Climate => GaugeManager.Instance.ClimateGauge,
            EventData.EventCategory.Societal => GaugeManager.Instance.SocietalGauge,
            EventData.EventCategory.Trust => GaugeManager.Instance.TrustGauge,
            _ => null
        };
        
        if (gauge == null) return false;
        
        return gauge.value >= MinValue && gauge.value <= MaxValue;
    }
}
