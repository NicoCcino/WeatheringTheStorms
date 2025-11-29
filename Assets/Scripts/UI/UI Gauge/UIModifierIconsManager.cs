using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UIModifierIconsManager : MonoBehaviour
{
    [SerializeField] private GameObject eventModifierIconPrefab;
    [SerializeField] private UIModifierIcon upgradeModifierIcon;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;
    [SerializeField] private bool isClimate = false;
    [SerializeField] private bool isSocietal = false;
    [SerializeField] private bool isHuman = false;

    //HACK : just to fix human upgrades
    private int humanUpgradeCount;
    private void OnEnable()
    {
        EventManager.Instance.OnEventTriggered += OnEventTriggeredCallback;
        Upgrade.OnAnyUpgradeBought += OnAnyUpgradeBoughtCallback;
    }

    private void OnAnyUpgradeBoughtCallback(Upgrade upgrade)
    {
        float addedValue = 0.0f;
        if (isClimate) addedValue = upgrade.UpgradeData.ModifierBank.ClimateModifier.AddedValue;
        if (isHuman) addedValue = upgrade.UpgradeData.ModifierBank.HumanModifier.AddedValue;
        if (isSocietal) addedValue = upgrade.UpgradeData.ModifierBank.SocietalModifier.AddedValue;

        if (addedValue == 0.0f)
        {
            return;
        }
        upgradeModifierIcon.gameObject.SetActive(true);
        if (!isHuman)
            upgradeModifierIcon.IncrementUpgradeAddedValue(addedValue);
        else
        {
            humanUpgradeCount++;
            if (humanUpgradeCount >= 2)
            {
                upgradeModifierIcon.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEventTriggered -= OnEventTriggeredCallback;
        Upgrade.OnAnyUpgradeBought -= OnAnyUpgradeBoughtCallback;
    }
    private void OnEventTriggeredCallback(Event ev)
    {
        if (ev.EventData.Duration <= 0)
            return;


        float addedValue = 0.0f;
        if (isClimate) addedValue = ev.EventData.ModifierBank.ClimateModifier.AddedValue;
        if (isHuman) addedValue = ev.EventData.ModifierBank.HumanModifier.AddedValue;
        if (isSocietal) addedValue = ev.EventData.ModifierBank.SocietalModifier.AddedValue;
        if (addedValue == 0.0f)
        {
            return;
        }

        DisplayModifierIconFromEvent(ev, addedValue);
    }

    public void DisplayModifierIconFromEvent(Event ev, float addedValue)
    {
        GameObject go = SimplePool.Spawn(eventModifierIconPrefab);
        UIModifierIcon uIModifierIcon = go.GetComponent<UIModifierIcon>();
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
        Color color = ev.IsEventPositive() ? positiveColor : negativeColor;
        uIModifierIcon.DisplayModifier(ev.EventData.Icon, addedValue, (uint)ev.EventData.Duration, color, ev.EventData.Description);
    }
}
