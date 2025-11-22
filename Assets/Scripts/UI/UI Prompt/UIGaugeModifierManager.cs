using UnityEngine;

public class UIGaugeModifierManager : MonoBehaviour
{
    [SerializeField] private UIGaugeModifier gaugeModifierHuman;
    [SerializeField] private UIGaugeModifier gaugeModifierSocietal;
    [SerializeField] private UIGaugeModifier gaugeModifierClimate;

    [SerializeField] private AnimationCurve curveAmountScaling;

    public void DisplayModifierBank(ModifierBank modifierBank)
    {
        UpdateUIGauge(gaugeModifierHuman, modifierBank.HumanModifier);
        UpdateUIGauge(gaugeModifierSocietal, modifierBank.SocietalModifier);
        UpdateUIGauge(gaugeModifierClimate, modifierBank.ClimateModifier);
    }

    private void UpdateUIGauge(UIGaugeModifier uiGauge, Modifier modifier)
    {
        bool isDisplayed = modifier.AddedValue != "";
        uiGauge.gameObject.SetActive(isDisplayed);

        if (isDisplayed)
            uiGauge.DisplayModifier(curveAmountScaling, modifier);
    }
}

