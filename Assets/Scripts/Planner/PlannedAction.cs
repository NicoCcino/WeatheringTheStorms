using UnityEngine;

/// <summary>
/// Represents a planned action that can be scheduled after a Prompt or Event is triggered
/// </summary>
[System.Serializable]
public class PlannedAction
{
    [field: SerializeField, Tooltip("Number of ticks to wait before triggering the planned action")]
    public uint TicksDelay { get; private set; }

    [field: SerializeField, Tooltip("The prompt to trigger after the delay (mutually exclusive with PlannedEvent)")]
    public Prompt PlannedPrompt { get; private set; }

    [field: SerializeField, Tooltip("The event to trigger after the delay (mutually exclusive with PlannedPrompt)")]
    public Event PlannedEvent { get; private set; }

    /// <summary>
    /// Check if this planned action is valid (has either a prompt or event)
    /// </summary>
    public bool IsValid()
    {
        return PlannedPrompt != null || PlannedEvent != null;
    }

    /// <summary>
    /// Schedule this planned action using the Planner
    /// </summary>
    public void Schedule(uint currentTick)
    {
        if (!IsValid())
        {
            Debug.LogWarning("Attempted to schedule an invalid PlannedAction (no prompt or event)");
            return;
        }

        uint triggerTick = currentTick + TicksDelay;

        if (PlannedPrompt != null)
        {
            Planner.Instance.SchedulePrompt(PlannedPrompt, triggerTick);
        }
        else if (PlannedEvent != null)
        {
            Planner.Instance.ScheduleEvent(PlannedEvent, triggerTick);
        }
    }
}

