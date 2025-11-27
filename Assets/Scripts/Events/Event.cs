using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Event", fileName = "Event")]
public class Event : ScriptableObject, IGridObject
{
    public EventData EventData;
    public Vector2Int Coordinates { get => EventData.Coordinates; set => EventData.Coordinates = value; }
    public Action<Event> OnEventTriggered;
    public Action<Event> OnEventHoveredEnter;
    public Action<Event> OnEventHoveredExit;
    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke(this);
    }

    public bool IsEventPositive()
    {
        float totalBalance = 0;

        totalBalance += EventData.ModifierBank.ClimateModifier.AddedValue;
        totalBalance += EventData.ModifierBank.SocietalModifier.AddedValue;
        totalBalance += EventData.ModifierBank.HumanModifier.AddedValue;

        return totalBalance >= 0;
    }
}

