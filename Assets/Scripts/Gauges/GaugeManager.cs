using System;
using UnityEngine;

public class GaugeManager : Singleton<GaugeManager>
{
    [SerializeField] public Gauge ClimateGauge;
    [SerializeField] public Gauge SocietalGauge;
    [SerializeField] public Gauge TrustGauge;

    private void Start()
    {
        ClimateGauge.Init();
        SocietalGauge.Init();
        TrustGauge.Init();

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

    private void OnTimelineTick(int currentTick)
    {
        float humanImpact = Human.Instance.GetHumanImpactOnGauges();
        ClimateGauge.OnTimelineTick(currentTick, humanImpact);
        SocietalGauge.OnTimelineTick(currentTick, humanImpact);
        TrustGauge.OnTimelineTick(currentTick, humanImpact);

        // Check if we win
        if (ClimateGauge.value >= ClimateGauge.gaugeParameter.Max &&
            SocietalGauge.value >= SocietalGauge.gaugeParameter.Max &&
            TrustGauge.value >= TrustGauge.gaugeParameter.Max)
        {
            Debug.Log("All gauges have reached their maximum values!\nYou win motherfucker!");
        }
    }

    public void ApplyModifierBank(ModifierBank modifierBank)
    {
        ClimateGauge.AddModifier(modifierBank.ClimateModifier);
        SocietalGauge.AddModifier(modifierBank.SocietalModifier);
        TrustGauge.AddModifier(modifierBank.TrustModifier);
        Human.Instance.AddModifier(modifierBank.HumanModifier);
    }
}
