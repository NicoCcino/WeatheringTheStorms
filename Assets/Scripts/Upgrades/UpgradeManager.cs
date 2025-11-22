using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [Header("Upgrades List")]
    [SerializeField] public Upgrade[] AllUpgrades;

    private void OnEnable()
    {
        RefreshUpgrades();
    }
    public void RefreshUpgrades()
    {
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
    }
}
