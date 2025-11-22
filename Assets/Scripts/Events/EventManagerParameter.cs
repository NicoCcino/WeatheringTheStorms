using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/EventManagerParameter", fileName = "EventManagerParameter")]
public class EventManagerParameter : ScriptableObject
{
    public List<Event> AllEvents;
    public AnimationCurve EventProbabilityOverTicks;

}

