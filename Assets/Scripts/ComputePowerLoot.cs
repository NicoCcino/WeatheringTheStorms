using UnityEngine;

[System.Serializable]
public class ComputePowerLootData : IGridObject
{
    public ComputePowerLootData(int amount)
    {
        Amount = amount;
    }
    public int Amount { get; set; }
    public Vector2Int Coordinates { get; set; }
}
