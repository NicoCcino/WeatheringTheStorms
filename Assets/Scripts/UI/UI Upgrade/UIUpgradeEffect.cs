using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeEffect : MonoBehaviour
{
    [SerializeField] private Image iconGauge;
    [SerializeField] private TextMeshProUGUI textEffect;

    public void DIsplayFromModifier(Sprite icon, Modifier modifier)
    {
        iconGauge.sprite = icon;
        textEffect.text = (modifier.AddedValue > 0 ? "+" : "") + (modifier.AddedValue * 10).ToString("0.#");
    }
}


