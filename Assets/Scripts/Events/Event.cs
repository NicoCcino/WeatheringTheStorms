using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Event", fileName = "Event")]
public class Event : ScriptableObject, IGridObject
{
    public EventData EventData;
    [field: SerializeField] public Vector2Int Coordinates { get; set; }
    public Action<Event> OnEventTriggered;
    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke(this);
    }
}

