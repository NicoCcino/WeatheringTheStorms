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
        ClimateGauge.OnTimelineTick(currentTick);
        SocietalGauge.OnTimelineTick(currentTick);
        TrustGauge.OnTimelineTick(currentTick);
    }

    public void ApplyModifierBank(ModifierBank modifierBank)
    {
        ClimateGauge.AddModifier(modifierBank.ClimateModifier);
        SocietalGauge.AddModifier(modifierBank.SocietalModifier);
        TrustGauge.AddModifier(modifierBank.TrustModifier);
    }
}
