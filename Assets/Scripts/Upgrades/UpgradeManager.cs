using System;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [Header("Upgrades List")]
    [SerializeField] public Upgrade[] AllUpgrades;

    [SerializeField] public Sprite climateSprite;
    [SerializeField] public Sprite societalSprite;
    [SerializeField] public Sprite humanSprite;
    [SerializeField] public Sprite trustSprite;

    private void Start()
    {
        ComputePower.Instance.OnCP += RefreshUpgrades;
        Upgrade.OnAnyUpgradeBought += OnAnyUpgradeBoughtCallback;
    }

    private void OnAnyUpgradeBoughtCallback(Upgrade upgrade)
    {
        RefreshUpgrades(0);
    }

    public void RefreshUpgrades(int _)
    {
        Debug.Log("refresh upgrades");
        foreach (var u in AllUpgrades)
        {
            u.TryUnlock();
        }
    }
    private void ResetAllUpgrades()
    {
        foreach (var upgrade in AllUpgrades)
        {
            upgrade.ResetStatus();
        }
    }
    private void OnDisable()
    {
        ResetAllUpgrades();
        ComputePower.Instance.OnCP -= RefreshUpgrades;
        Upgrade.OnAnyUpgradeBought -= OnAnyUpgradeBoughtCallback;
    }
}
