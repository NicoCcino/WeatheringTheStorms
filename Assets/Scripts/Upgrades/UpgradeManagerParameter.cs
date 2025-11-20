using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/UpgradeManagerParameter", fileName = "UpgradeManagerParameter")]
public class UpgradeManagerParameter : ScriptableObject
{
    public List<Upgrade> AllUpgrades;
}
