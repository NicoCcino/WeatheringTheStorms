using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI modifierText;
    public void UpdateGauge(float value)
    {
        slider.value = value;

    }
    public void UpdateModifier(float value)
    {
        if (value > 0)
        {
            modifierText.color = Color.green;   // positif
            modifierText.text = "+" + value;
        }
        else if (value < 0)
        {
            modifierText.color = Color.red;     // négatif 
            modifierText.text = value.ToString();
        }

        else
        {
            modifierText.color = Color.white;   // neutre
            modifierText.text = "+" + value;
        }

    }
}
