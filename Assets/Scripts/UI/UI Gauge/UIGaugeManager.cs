using System.Threading.Tasks;
using UnityEngine;

public class UIGaugeManager : MonoBehaviour
{
        [SerializeField] private UIGauge uiGaugeClimate;
        [SerializeField] private UIGauge uiGaugeSocietal;
        [SerializeField] private UIGauge uiGaugeTrust;

        private void OnEnable()
        {
                GaugeManager.Instance.ClimateGauge.OnGaugeModified += (x) => uiGaugeClimate.UpdateGauge(x);
                GaugeManager.Instance.SocietalGauge.OnGaugeModified += (x) => uiGaugeSocietal.UpdateGauge(x);
                GaugeManager.Instance.TrustGauge.OnGaugeModified += (x) => uiGaugeTrust.UpdateGauge(x);

                //Timeline.Instance.OnTick += UpdateDeltaUIGauges;
        }
        private async void UpdateDeltaUIGauges(uint _)
        {
                await Task.Yield();
                uiGaugeClimate.UpdateDeltaGauge();
                uiGaugeSocietal.UpdateDeltaGauge();
                uiGaugeTrust.UpdateDeltaGauge();
        }

        private void Update()
        {
                uiGaugeClimate.UpdateModifier(Human.Instance.HumanImpact);
                uiGaugeSocietal.UpdateModifier(Human.Instance.HumanImpact);
                uiGaugeTrust.UpdateModifier(GaugeManager.Instance.ClimateGauge.iterationValue);
        }
}
