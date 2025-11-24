using System.Linq;
using UnityEngine;

public class UIGaugeModifierManager : MonoBehaviour
{
    [SerializeField] private UIGaugeModifier gaugeModifierTrust;
    [SerializeField] private UIGaugeModifier gaugeModifierSocietal;
    [SerializeField] private UIGaugeModifier gaugeModifierClimate;

    [SerializeField] private AnimationCurve curveAmountScaling;

    public void DisplayModifierBank(ModifierBank modifierBank)
    {
        UpdateUIGauge(gaugeModifierSocietal, modifierBank.SocietalModifier);
        UpdateUIGauge(gaugeModifierClimate, modifierBank.ClimateModifier);
        UpdateUIGauge(gaugeModifierTrust, modifierBank.TrustModifier);
    }

    private void UpdateUIGauge(UIGaugeModifier uiGauge, Modifier modifier)
    {
        bool isDisplayed = modifier.OneShotValue != 0;
        uiGauge.gameObject.SetActive(isDisplayed);
        if (!isDisplayed) return;
        uiGauge.DisplayModifier(curveAmountScaling, modifier);
    }
}

