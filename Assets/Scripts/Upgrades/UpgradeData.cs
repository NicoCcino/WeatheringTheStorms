using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    [field: Header("Displayed Informations")]
    [field: SerializeField, Tooltip("Upgrade name")] public string Label { get; private set; }
    [field: SerializeField, Tooltip("Upgrade description"), TextArea(1, 4)] public string Description { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: Header("Hidden Informations")]
    [field: SerializeField] public ModifierBank ModifierBank { get; private set; }

}
