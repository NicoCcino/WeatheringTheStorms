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
        string text = value.ToString("0.#");
        if (value > 0)
        {

            modifierText.color = Color.green;   // positif
            modifierText.text = "+" + text;
        }
        else if (value < 0)
        {
            modifierText.color = Color.red;     // négatif 
            modifierText.text = text;
        }

        else
        {
            modifierText.color = Color.white;   // neutre
            modifierText.text = "+" + text;
        }

    }
}
