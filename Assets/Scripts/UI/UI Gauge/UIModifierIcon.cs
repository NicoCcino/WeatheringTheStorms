using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIModifierIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textModifierValue;
    [SerializeField] private Image modifierIcon;
    [SerializeField] private Image imageDuration;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private UIHoverInformation uiHoverInformation;
    private float remainingTime;
    private uint duration;
    private uint startTick;
    public void DisplayModifier(Sprite icon, float addedValue, uint duration, Color color, string description = null)
    {
        if (description != null)
        {
            uiHoverInformation.DisplayedText = description;
        }
        startTick = Timeline.Instance.CurrentTick;
        backgroundImage.color = color;
        this.duration = duration;
        remainingTime = duration;
        modifierIcon.sprite = icon;
        imageDuration.fillAmount = 1;
        textModifierValue.text = (addedValue * 10).ToString("0.#");
    }
    //Specific Upgrade
    public float totalUpgradeAddedValue = 0.0f;
    public void IncrementUpgradeAddedValue(float addedValue)
    {
        totalUpgradeAddedValue += addedValue;
        textModifierValue.text = "+" + (totalUpgradeAddedValue * 10).ToString("0.#");
    }//
    private void Update()
    {
        if (duration <= 0) return;
        float delta = (Time.deltaTime * Timeline.Instance.tickFreq);
        remainingTime -= delta;
        imageDuration.fillAmount = remainingTime / (float)duration;
        if (remainingTime <= 0)
        {
            SimplePool.Despawn(gameObject);
        }
    }

}
