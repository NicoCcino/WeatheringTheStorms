using UnityEngine;

public class Upgrade
{
    public UpgradeData UpgradeData;
    public bool IsUnlocked = false;

    public Upgrade(UpgradeData upgradeData)
    {
        if (upgradeData == null)
        {
            Debug.LogWarning("Upgrade: Upgrade data is not assigned!");
            return;
        }
        UpgradeData = upgradeData;
        IsUnlocked = false;
    }

    public void Unlock()
    {
        if (IsUnlocked == true)
        {
            Debug.LogWarning("Upgrade is already unlocked!");
            return;
        }
        IsUnlocked = true;
        // apply modifier bank to the gauges
        GaugeManager.Instance.ApplyModifierBank(UpgradeData.ModifierBank);
        //Debug.Log("Upgrade unlocked: " + UpgradeData.Label);
        LogFileManager.Instance.LogUserAction("Upgrade", UpgradeData.Label);
        // Spend compute power
        ComputePower.Instance.SpendComputePower(UpgradeData.Cost);
    }
}
