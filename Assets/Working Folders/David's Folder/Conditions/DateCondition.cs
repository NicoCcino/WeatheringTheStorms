using UnityEngine;

[System.Serializable]
public class DateCondition : ICondition
{
    [field: SerializeField] public int MinTick { get; private set; }
    [field: SerializeField] public int MaxTick { get; private set; }
    public DateCondition(int minTick, int maxTick)
    {
        MinTick = minTick;
        MaxTick = maxTick;
    }

    public bool IsFulfilled()
    {
        //TODO : Implementation =>  return Timeline.CurrentTick>= minTick && Timeline.CurrentTick <=maxTick;
        return false;
    }
}
