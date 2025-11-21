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
            Human.Instance.OnHumanCountChanged += OnHumanCountChanged;
        }
    }

    private void OnDisable()
    {
        if (Timeline.Instance != null)
        {
            Human.Instance.OnHumanCountChanged -= OnHumanCountChanged;
        }
    }

    private void OnHumanCountChanged(uint currentTick, float humanImpact)
    {
        ClimateGauge.OnHumanCountChanged(humanImpact);
        SocietalGauge.OnHumanCountChanged(humanImpact);
        TrustGauge.OnHumanCountChanged(humanImpact);

        // Check if we win
        if (ClimateGauge.value >= ClimateGauge.gaugeParameter.Max &&
            SocietalGauge.value >= SocietalGauge.gaugeParameter.Max &&
            TrustGauge.value >= TrustGauge.gaugeParameter.Max)
        {
            Debug.Log("All gauges have reached their maximum values!\nYou win motherfucker!");
        }
        OnGaugeChanged?.Invoke(currentTick);
    }

    public void ApplyModifierBank(ModifierBank modifierBank)
    {
        ClimateGauge.AddModifier(modifierBank.ClimateModifier);
        SocietalGauge.AddModifier(modifierBank.SocietalModifier);
        TrustGauge.AddModifier(modifierBank.TrustModifier);
        Human.Instance.AddModifier(modifierBank.HumanModifier);
    }
}
