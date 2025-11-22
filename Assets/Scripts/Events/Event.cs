using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Event", fileName = "Event")]
public class Event : ScriptableObject
{
    public EventData EventData;
    public Action<Event> OnEventTriggered;
    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke(this);
    }
}

