using System;
using UnityEngine;

public class GaugeManager : Singleton<GaugeManager>
{
    [SerializeField] public Gauge ClimateGauge;
    [SerializeField] public Gauge SocietalGauge;
    [SerializeField] public Gauge TrustGauge;

    /// <summary>
    /// Event triggered when the human count changes
    /// </summary>
    public Action<uint> OnGaugeChanged;

    private void Start()
    {
        ClimateGauge.Init();
        SocietalGauge.Init();
        TrustGauge.Init();
        // Subscribe to the OnTick event from Timeline
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnHumanCountChanged;
        }
    }

    private void OnDisable()
    {
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick -= OnHumanCountChanged;
        }
    }

    private void OnHumanCountChanged(uint currentTick)
    {
        ClimateGauge.OnTick();
        SocietalGauge.OnTick();
        TrustGauge.OnTick();

        OnGaugeChanged?.Invoke(currentTick);
    }

    public void ApplyModifierBank(ModifierBank modifierBank)
    {
        ClimateGauge.AddModifier(modifierBank.ClimateModifier);
        SocietalGauge.AddModifier(modifierBank.SocietalModifier);
        TrustGauge.AddModifier(modifierBank.TrustModifier);
    }
}
