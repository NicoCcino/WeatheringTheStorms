using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Upgrade", fileName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public UpgradeData UpgradeData;

    public bool IsValid => UpgradeData.Cost <= ComputePower.Instance.value;
    public bool IsUnlocked = false;

    public void Unlock()
    {
        IsUnlocked = true;
        // apply modifier bank to the gauges
        GaugeManager.Instance.ApplyModifierBank(UpgradeData.ModifierBank);
    }
}
