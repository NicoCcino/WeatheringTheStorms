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
}

