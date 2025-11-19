using System;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private void OnEnable()
    {
        Timeline.Instance.OnTick += OnTickCallback;
    }
    private void OnDisable()
    {
        Timeline.Instance.OnTick -= OnTickCallback;
    }
    private void OnTickCallback(int currentTick)
    {
        throw new NotImplementedException();
    }


}
