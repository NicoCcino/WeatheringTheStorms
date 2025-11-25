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
        if (Human.Instance != null)
        {
            Human.Instance.OnHumanCountChanged += OnHumanCountChanged;
        }
    }

    private void OnDisable()
    {
        // Subscribe to the OnTick event from Timeline
        if (Human.Instance != null)
        {
            Human.Instance.OnHumanCountChanged -= OnHumanCountChanged;
        }
    }

    private void OnHumanCountChanged(uint currentTick, float humanImpact)
    {
        ClimateGauge.OnHumanCountChanged(humanImpact);
        SocietalGauge.OnHumanCountChanged(humanImpact);
        TrustGauge.OnHumanCountChanged(humanImpact);

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
