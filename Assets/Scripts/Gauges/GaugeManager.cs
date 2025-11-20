using System;
using UnityEngine;

public class GaugeManager : Singleton<GaugeManager>
{
    [SerializeField] private Gauge ClimateGauge;
    [SerializeField] private Gauge SocietalGauge;
    [SerializeField] private Gauge TrustGauge;

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
    }

    public void ApplyModifierBank(ModifierBank modifierBank)
    {
        ClimateGauge.AddModifier(modifierBank.ClimateModifier);
        SocietalGauge.AddModifier(modifierBank.SocietalModifier);
        TrustGauge.AddModifier(modifierBank.TrustModifier);
        Human.Instance.AddModifier(modifierBank.HumanModifier);
    }
}
