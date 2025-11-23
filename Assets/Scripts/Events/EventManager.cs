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
    private HashSet<Event> TriggeredEvents { get; set; } = new HashSet<Event>();
    public Action<Event> OnEventTriggered;
    public Action<Event> OnEventOpened;
    private int noEventTickCounter = 0;
    private Event CurrentEvent = null;
    private void Start()
    {
        if (EventManagerParameter == null)
        {
            Debug.LogError("EventManagerParameter is not assigned in the EventManager! Please assign it in the Unity Inspector.");
            return;
        }

        if (EventManagerParameter != null)
        {
            AvailableEvents = new List<Event>(EventManagerParameter.AllEvents);
        }
        // Subscribe to the OnTick action from Timeline
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnTickCallback;
        }
    }
    private void OnDisable()
    {
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick -= OnTickCallback;
        }
    }
    private void OnTickCallback(uint currentTick)
    {
        //We roll the dices to see if we should trigger a event on this tick
        bool shouldAnyEventOccur = ShouldAnyEventOccur();
        if (shouldAnyEventOccur == false)
        {
            noEventTickCounter++;
            return;
        }

        //We select a random event in all available events 
        Event selectedEvent = PickRandomValidEvent();
        if (selectedEvent == null) return;
        //Then we're ready to trigger all following logics linked to the event
        TriggerEvent(selectedEvent);
    }

    private Event PickRandomValidEvent()
    {
        // Create a list of valid events we will filter
        Event[] validEvents = AvailableEvents.ToArray();

        // We filter the remaining events to match the current date condition
        validEvents = validEvents.Where(e => e.EventData.DateCondition.IsFulfilled()).ToArray();

        // We filter the remaining events to match the current gauge conditions
        validEvents = validEvents.Where(e => e.EventData.GaugeCondition.IsFulfilled()).ToArray();

        // We filter the remaining events to match the parent event condition
        validEvents = validEvents.Where(e => IsParentEventTriggered(e)).ToArray();

        if (validEvents.Length == 0)
        {
            Debug.LogError("There is no available Event anymore. We should create more Events to ensure there is always enough events in the game or reduce the frequency of events");
            LogFileManager.Instance.LogUserAction("Warning", "There is no available Event anymore. We should create more Events to ensure there is always enough events in the game or reduce the frequency of events");
            return null;
        }

        int random = UnityEngine.Random.Range(0, validEvents.Length);
        return validEvents[random];
    }

    private bool IsParentEventTriggered(Event eventToCheck)
    {
        // If no parent is set, the event is always valid
        if (eventToCheck.EventData.ParentEvent == null)
            return true;

        // Check if the parent event has been triggered
        return TriggeredEvents.Contains(eventToCheck.EventData.ParentEvent);
    }

    private bool ShouldAnyEventOccur()
    {
        if (EventManagerParameter == null)
        {
            Debug.LogError("EventManagerParameter is not assigned in the EventManager! Please assign it in the Unity Inspector.");
            return false;
        }

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        float probability = EventManagerParameter.EventProbabilityOverTicks.Evaluate(noEventTickCounter);

        return randomValue < probability;
    }

    public void TriggerEvent(Event triggeredEvent)
    {
        //We remove the selected event from the available events (so it cant occur twice in a game)
        AvailableEvents.Remove(triggeredEvent);
        //We add the event to the triggered events history
        TriggeredEvents.Add(triggeredEvent);
        noEventTickCounter = 0;
        Debug.Log($"Event {triggeredEvent.EventData.Description} Triggered");
        LogFileManager.Instance.LogUserAction("Event", triggeredEvent.EventData.Description);
        OnEventTriggered?.Invoke(triggeredEvent);
        CurrentEvent = triggeredEvent;
        triggeredEvent.OnEventTriggered += OnCurrentEventTriggered;
        // Apply the modifier bank to the gauges
        GaugeManager.Instance.ApplyModifierBank(triggeredEvent.EventData.ModifierBank);
    }

    private void OnCurrentEventTriggered(Event triggeredEvent)
    {
        if (CurrentEvent == null) return;

        Timeline.Instance.SetPlaySpeed();
        CurrentEvent.OnEventTriggered -= OnCurrentEventTriggered;
        CurrentEvent = null;
    }
    public void OpenEvent(Event ev)
    {
        OnEventOpened?.Invoke(ev);
    }
}

