using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIModifierIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textModifierValue;
    [SerializeField] private Image modifierIcon;
    [SerializeField] private Image imageDuration;

    [SerializeField] private UIHoverInformation uiHoverInformation;
    private uint duration;
    private uint startTick;
    public void DisplayModifier(Sprite icon, float addedValue, uint duration, string description = null)
    {
        if (description != null)
        {
            uiHoverInformation.DisplayedText = description;
        }
        startTick = Timeline.Instance.CurrentTick;
        this.duration = duration;
        modifierIcon.sprite = icon;
        imageDuration.fillAmount = 1;
        textModifierValue.text = (addedValue * 10).ToString("0");
    }
    //Specific Upgrade
    public float totalUpdateAddedValue = 0.0f;
    public void IncrementUpgradeAddedValue(float addedValue)
    {
        totalUpdateAddedValue += addedValue;
        textModifierValue.text = "+" + totalUpdateAddedValue.ToString("0");
    }//
    private void OnEnable()
    {
        Timeline.Instance.OnTick += OnTickCallback;
    }
    private void OnDisable()
    {
        Timeline.Instance.OnTick -= OnTickCallback;
    }
    private void OnTickCallback(uint currentTick)
    {
        if (duration <= 0) return;

        uint remainingTime = duration - (currentTick - startTick);
        imageDuration.fillAmount = remainingTime / (float)duration;

        if (remainingTime <= 0)
        {
            SimplePool.Despawn(gameObject);
        }
    }

}
