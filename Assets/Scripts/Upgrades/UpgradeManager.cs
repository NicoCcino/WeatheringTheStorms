using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private List<Upgrade> AvailableUpgrades { get; set; } = new List<Upgrade>();
    public UpgradeManagerParameter UpgradeManagerParameter;

    private void Start()
    {
        foreach (var upgrade in UpgradeManagerParameter.AllUpgrades)
        {
            AvailableUpgrades.Add(upgrade);
        }
    }

    public void buyUpgrade(Upgrade upgrade)
    {
        if (upgrade.IsValid == false)
        {
            Debug.LogError("Upgrade is not valid!");
            return;
        }
        ComputePower.Instance.SpendComputePower(upgrade.UpgradeData.Cost);
        upgrade.Unlock();
    }

    public List<Upgrade> GetAvailableUpgrades()
    {
        return AvailableUpgrades.Where(u => u.IsValid).ToList();
    }

    public List<Upgrade> GetUnlockedUpgrades()
    {
        return AvailableUpgrades.Where(u => u.IsUnlocked).ToList();
    }

    public List<Upgrade> GetLockedUpgrades()
    {
        return AvailableUpgrades.Where(u => u.IsUnlocked == false).ToList();
    }

    public List<Upgrade> GetAllUpgrades()
    {
        return AvailableUpgrades;
    }
}
