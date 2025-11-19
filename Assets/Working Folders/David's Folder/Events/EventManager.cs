using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manage Events triggering based on <see cref="Timeline"/> ticks and <see cref="EventManagerParameter"/> 
/// </summary>
public class EventManager : Singleton<EventManager>
{
    public EventManagerParameter EventManagerParameter;
    private List<Event> AvailableEvents { get; set; } = new List<Event>();
    public Action<Event> OnEventTriggered;
    private int noEventTickCounter = 0;
    private void OnEnable()
    {
        AvailableEvents = EventManagerParameter.AllEvents;
        Timeline.Instance.OnTick += OnTickCallback;
    }
    private void OnDisable()
    {
        Timeline.Instance.OnTick -= OnTickCallback;
    }
    private void OnTickCallback(int currentTick)
    {
        //We roll the dices to see if we should trigger an event on this tick
        bool shouldAnEventOccur = ShouldAnyEventOccur();
        if (shouldAnEventOccur == false)
        {
            noEventTickCounter++;
            return;
        }

        //We select a random event in all availables events 
        Event ev = PickRandomValidEvent();
        //We remove the selected event from the available events (so it cant occur twice in a game)
        AvailableEvents.Remove(ev);
        //Then we're ready to trigger all following logics linked to the event
        TriggerEvent(ev);
    }

    private Event PickRandomValidEvent()
    {
        Event[] validEvents = AvailableEvents.Where(e => e.IsValid).ToArray();

        int random = UnityEngine.Random.Range(0, validEvents.Length);
        return validEvents[random];
    }
    private bool ShouldAnyEventOccur()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        float probability = EventManagerParameter.EventProbabilityOverTicks.Evaluate(noEventTickCounter);

        return randomValue < probability;
    }

    public void TriggerEvent(Event ev)
    {
        noEventTickCounter = 0;
        OnEventTriggered?.Invoke(ev);
    }
}
