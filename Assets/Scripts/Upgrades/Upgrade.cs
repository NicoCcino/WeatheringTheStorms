using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Game/Upgrade")]
public class Upgrade : ScriptableObject
{
    public static Action<Upgrade> OnAnyUpgradeBought;


    [Header("Datas & Balancing")]
    public UpgradeData UpgradeData;

    [Header("Status")]
    public bool IsUnlocked = false;
    public bool IsBought = false;

    public Action OnUnlocked;
    public Action OnBought;


    [field: Header("Upgrade hierarchy")]
    [field: SerializeField] public Upgrade[] ParentUpgrades;

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

    public bool TryUnlock()
    {
        if (IsUnlocked) return true;
        RefreshIsUnlocked();
        return IsUnlocked;
    }
    public void Buy()
    {
        if (!IsUnlocked) return;

        GaugeManager.Instance.ApplyModifierBank(UpgradeData.ModifierBank);
        //Debug.Log("Upgrade unlocked: " + UpgradeData.Label);
        LogFileManager.Instance.LogUserAction("Upgrade", UpgradeData.Label);
        // Spend compute power
        ComputePower.Instance.SpendComputePower(UpgradeData.Cost);

        IsBought = true;

        OnBought?.Invoke();
        OnAnyUpgradeBought?.Invoke(this);
    }
    public void Sell()
    {
        IsBought = false;
        //TODO : Implement
    }
    private void RefreshIsUnlocked()
    {
        if (ParentUpgrades.Any(up => up.IsBought) || ParentUpgrades.Length == 0)
        {
            IsUnlocked = true;
            OnUnlocked?.Invoke();
        }
    }
    public bool IsBuyable()
    {
        bool enoughCp = ComputePower.Instance.value >= UpgradeData.Cost;
        return enoughCp && IsUnlocked && !IsBought;
    }
    public void ResetStatus()
    {
        IsUnlocked = false;
        IsBought = false;
    }
}
