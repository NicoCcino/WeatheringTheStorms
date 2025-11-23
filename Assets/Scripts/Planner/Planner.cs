using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages scheduled Prompts and Events that will be triggered at specific ticks
/// </summary>
public class Planner : Singleton<Planner>
{
    // Queue of scheduled actions sorted by tick
    private SortedList<uint, List<ScheduledAction>> scheduledActions = new SortedList<uint, List<ScheduledAction>>();

    /// <summary>
    /// Schedule a prompt to be triggered at a specific tick
    /// </summary>
    public void SchedulePrompt(Prompt prompt, uint triggerTick)
    {
        ScheduleAction(new ScheduledAction(ScheduledActionType.Prompt, prompt, null, triggerTick));
        Debug.Log($"Scheduled Prompt '{prompt.PromptData.Label}' for tick {triggerTick}");
    }

    /// <summary>
    /// Schedule an event to be triggered at a specific tick
    /// </summary>
    public void ScheduleEvent(Event evt, uint triggerTick)
    {
        ScheduleAction(new ScheduledAction(ScheduledActionType.Event, null, evt, triggerTick));
        Debug.Log($"Scheduled Event '{evt.EventData.Description}' for tick {triggerTick}");
    }

    /// <summary>
    /// Internal method to add a scheduled action to the queue
    /// </summary>
    private void ScheduleAction(ScheduledAction action)
    {
        if (!scheduledActions.ContainsKey(action.TriggerTick))
        {
            scheduledActions[action.TriggerTick] = new List<ScheduledAction>();
        }
        scheduledActions[action.TriggerTick].Add(action);
    }

    /// <summary>
    /// Check if there are any actions scheduled for the current tick
    /// </summary>
    public bool HasScheduledActionsForTick(uint tick)
    {
        return scheduledActions.ContainsKey(tick) && scheduledActions[tick].Count > 0;
    }

    /// <summary>
    /// Get and remove scheduled actions for the current tick, optionally filtered by type
    /// </summary>
    public List<ScheduledAction> GetAndConsumeScheduledActions(uint tick, ScheduledActionType? filterType = null)
    {
        if (scheduledActions.ContainsKey(tick))
        {
            if (filterType.HasValue)
            {
                // Filter and remove only the matching type
                List<ScheduledAction> matchingActions = scheduledActions[tick]
                    .Where(a => a.ActionType == filterType.Value)
                    .ToList();
                
                // Remove the matching actions from the list
                foreach (var action in matchingActions)
                {
                    scheduledActions[tick].Remove(action);
                }
                
                // If no more actions for this tick, remove the tick entry
                if (scheduledActions[tick].Count == 0)
                {
                    scheduledActions.Remove(tick);
                }
                
                return matchingActions;
            }
            else
            {
                // Original behavior - get all and remove
                List<ScheduledAction> actions = new List<ScheduledAction>(scheduledActions[tick]);
                scheduledActions.Remove(tick);
                return actions;
            }
        }
        return new List<ScheduledAction>();
    }
}

/// <summary>
/// Represents an action scheduled to occur at a specific tick
/// </summary>
[System.Serializable]
public class ScheduledAction
{
    public ScheduledActionType ActionType;
    public Prompt ScheduledPrompt;
    public Event ScheduledEvent;
    public uint TriggerTick;

    public ScheduledAction(ScheduledActionType actionType, Prompt prompt, Event evt, uint triggerTick)
    {
        ActionType = actionType;
        ScheduledPrompt = prompt;
        ScheduledEvent = evt;
        TriggerTick = triggerTick;
    }
}

/// <summary>
/// Type of scheduled action
/// </summary>
public enum ScheduledActionType
{
    Prompt,
    Event
}

