using UnityEngine;

public class UIGaugeManager : MonoBehaviour
{
    [SerializeField] private UIGauge uiGaugeClimate;
    [SerializeField] private UIGauge uiGaugeSocietal;
    [SerializeField] private UIGauge uiGaugeTrust;

    private void Update()
    {
        uiGaugeClimate.UpdateGauge(GaugeManager.Instance.ClimateGauge.value);
        uiGaugeSocietal.UpdateGauge(GaugeManager.Instance.TrustGauge.value);
        uiGaugeTrust.UpdateGauge(GaugeManager.Instance.SocietalGauge.value);
    }
}
