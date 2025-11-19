using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Event", fileName = "Event")]
public class Event : ScriptableObject
{
    public EventData EventData;

    public bool IsValid => EventData.DateCondition.IsFulfilled();
}
