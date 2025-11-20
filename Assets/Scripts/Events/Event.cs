using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Event", fileName = "Event")]
public class Event : ScriptableObject
{
    public EventData EventData;
    public Action<Choice> OnSolved;
    public void Solve(Choice choice)
    {
        OnSolved?.Invoke(choice);
    }
}
