using UnityEngine;
using UnityEngine.UI;
public class UIGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public void UpdateGauge(float value)
    {
        slider.value = value;
    }
}
