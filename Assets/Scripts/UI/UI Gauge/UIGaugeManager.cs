using UnityEngine;

public class UIGaugeManager : MonoBehaviour
{
    [SerializeField] private UIGauge uiGaugeClimate;
    [SerializeField] private UIGauge uiGaugeSocietal;

    private void Update()
    {
        uiGaugeClimate.UpdateGauge(GaugeManager.Instance.ClimateGauge.value);
        uiGaugeSocietal.UpdateGauge(GaugeManager.Instance.SocietalGauge.value);
    }
}
