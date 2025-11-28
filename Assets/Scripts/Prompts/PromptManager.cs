using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manage Prompts triggering based on <see cref="Timeline"/> ticks and <see cref="PromptManagerParameter"/> 
/// </summary>
public class PromptManager : Singleton<PromptManager>
{
    public PromptManagerParameter PromptManagerParameter;
    private List<Prompt> AvailablePrompts { get; set; } = new List<Prompt>();
    private Dictionary<Prompt, Choice> TriggeredPrompts { get; set; } = new Dictionary<Prompt, Choice>();
    public Action<Prompt> OnPromptSpawned;
    public Action<Prompt> OnPromptOpened;
    private int noPromptTickCounter = 0;
    private Prompt CurrentPrompt = null;
    private void Start()
    {
        if (PromptManagerParameter == null)
        {
            Debug.LogError("PromptManagerParameter is not assigned in the PromptManager! Please assign it in the Unity Inspector.");
            return;
        }

        if (PromptManagerParameter != null)
        {
            AvailablePrompts = new List<Prompt>(PromptManagerParameter.AllPrompts);
        }
        // Subscribe to the OnTick action from Timeline
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnTickCallback;
        }
        if (Planner.Instance == null)
        {
            Debug.LogError("Planner is not running. Please move the Planner component to a GameObject.");
            return;
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
        // First, check if there are any scheduled prompts for this tick
        if (Planner.Instance != null && Planner.Instance.HasScheduledActionsForTick(currentTick))
        {
            var scheduledActions = Planner.Instance.GetAndConsumeScheduledActions(currentTick, ScheduledActionType.Prompt);
            foreach (var action in scheduledActions)
            {
                if (action.ScheduledPrompt != null && AvailablePrompts.Contains(action.ScheduledPrompt))
                {
                    TriggerPrompt(action.ScheduledPrompt);
                }
            }
            if (scheduledActions.Count > 0)
            {
                return; // Scheduled prompts reset the random probability counter
            }
        }

        //We roll the dices to see if we should trigger a prompt on this tick
        bool shouldAnyPromptOccur = ShouldAnyPromptOccur();
        if (shouldAnyPromptOccur == false)
        {
            noPromptTickCounter++;
            return;
        }

        //We select a random prompt in all available prompts 
        Prompt prompt = PickRandomValidPrompt();
        if (prompt == null) return;
        //Then we're ready to trigger all following logics linked to the prompt
        TriggerPrompt(prompt);
    }

    private Prompt PickRandomValidPrompt()
    {
        // Create a list of valid prompts we will filter
        Prompt[] validPrompts = AvailablePrompts.ToArray();

        // We filter the remaining prompts to match the current date condition
        validPrompts = validPrompts.Where(p => p.PromptData.DateCondition.IsFulfilled()).ToArray();

        // We filter the remaining prompts to match the current gauge conditions
        validPrompts = validPrompts.Where(p => p.PromptData.GaugeCondition.IsFulfilled()).ToArray();

        // We filter the remaining prompts to match the parent prompt condition
        validPrompts = validPrompts.Where(p => IsParentPromptTriggered(p)).ToArray();

        if (validPrompts.Length == 0)
        {
            Debug.LogError("There is no available Prompt anymore. We should create more Prompts to ensure there is always enough prompts in the game or reduce the frequency of prompts");
            LogFileManager.Instance.LogUserAction("Warning", "There is no available Prompt anymore. We should create more Prompts to ensure there is always enough prompts in the game or reduce the frequency of prompts");
            return null;
        }

        int random = UnityEngine.Random.Range(0, validPrompts.Length);
        return validPrompts[random];
    }

    private bool IsParentPromptTriggered(Prompt promptToCheck)
    {
        // If no parent is set, the prompt is always valid
        if (promptToCheck.PromptData.ParentPrompt == null)
            return true;

        // Check if the parent prompt has been triggered
        return TriggeredPrompts.ContainsKey(promptToCheck.PromptData.ParentPrompt);
    }

    private bool ShouldAnyPromptOccur()
    {
        if (PromptManagerParameter == null)
        {
            Debug.LogError("PromptManagerParameter is not assigned in the PromptManager! Please assign it in the Unity Inspector.");
            return false;
        }

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        float probability = PromptManagerParameter.PromptProbabilityOverTicks.Evaluate(noPromptTickCounter);

        return randomValue < probability;
    }

    public void TriggerPrompt(Prompt prompt)
    {
        //We remove the selected prompt from the available prompts (so it cant occur twice in a game)
        AvailablePrompts.Remove(prompt);
        //We add the prompt to the triggered prompts history (with null choice until user answers)
        TriggeredPrompts[prompt] = null;
        noPromptTickCounter = 0;
        //Debug.Log($"Prompt {prompt.PromptData.Label} Triggered");
        OnPromptSpawned?.Invoke(prompt);
        CurrentPrompt = prompt;
        prompt.OnSolved += OnCurrentPromptSolved;
        //LogFileManager.Instance.LogUserAction("Prompt", prompt.PromptData.Label);
    }

    private void OnCurrentPromptSolved(Choice choice)
    {
        if (CurrentPrompt == null) return;

        // Store the user's choice for this prompt
        TriggeredPrompts[CurrentPrompt] = choice;

        // Log the choice to CSV file
        LogFileManager.Instance.LogUserAction("Choice", $"{CurrentPrompt.PromptData.Label}: {choice.Label}");

        Timeline.Instance.ResumeSpeed(false);
        CurrentPrompt.OnSolved -= OnCurrentPromptSolved;

        // Schedule any planned action if present
        if (choice.PlannedAction != null && choice.PlannedAction.IsValid())
        {
            if (Timeline.Instance != null && Planner.Instance != null)
            {
                choice.PlannedAction.Schedule(Timeline.Instance.CurrentTick);
            }
        }
    }
    public void OpenPrompt(Prompt prompt)
    {
        OnPromptOpened?.Invoke(prompt);
    }

    /// <summary>
    /// Get the choice that was made for a specific prompt. Returns null if prompt wasn't triggered or not yet answered.
    /// </summary>
    public Choice GetChoiceForPrompt(Prompt prompt)
    {
        return TriggeredPrompts.TryGetValue(prompt, out Choice choice) ? choice : null;
    }

    /// <summary>
    /// Check if a specific prompt was triggered (regardless of whether it was answered yet)
    /// </summary>
    public bool WasPromptTriggered(Prompt prompt)
    {
        return TriggeredPrompts.ContainsKey(prompt);
    }

    /// <summary>
    /// Check if a specific choice label was selected for a given prompt
    /// </summary>
    public bool WasChoiceMade(Prompt prompt, string choiceLabel)
    {
        if (TriggeredPrompts.TryGetValue(prompt, out Choice choice))
        {
            return choice != null && choice.Label == choiceLabel;
        }
        return false;
    }

    /// <summary>
    /// Get all triggered prompts with their choices (read-only)
    /// </summary>
    public Dictionary<Prompt, Choice> GetAllTriggeredPrompts()
    {
        return new Dictionary<Prompt, Choice>(TriggeredPrompts);
    }

    /// <summary>
    /// Get the conversation history for a prompt by traversing its parent chain.
    /// Only includes prompts with the same label (same conversation thread).
    /// Returns messages in chronological order (oldest first).
    /// </summary>
    public List<ChatMessage> GetConversationHistory(Prompt currentPrompt)
    {
        List<ChatMessage> history = new List<ChatMessage>();

        if (currentPrompt == null) return history;

        string currentLabel = currentPrompt.PromptData.Label;
        Prompt traversePrompt = currentPrompt.PromptData.ParentPrompt;

        // Traverse backwards through parent chain, collecting prompts with same label
        while (traversePrompt != null)
        {
            // Check if this parent has the same label
            if (traversePrompt.PromptData.Label == currentLabel)
            {
                // Get the choice that was made for this parent prompt
                Choice parentChoice = GetChoiceForPrompt(traversePrompt);

                if (parentChoice != null)
                {
                    // Insert at the beginning to maintain chronological order
                    // Insert the user's choice first (so it ends up after the prompt when both are inserted)
                    history.Insert(0, new ChatMessage(
                        "You",
                        parentChoice.Label,
                        ChatMessage.Alignment.Right
                    ));

                    // Insert the prompt message at the beginning (so it comes before the choice)
                    history.Insert(0, new ChatMessage(
                        traversePrompt.PromptData.Label,
                        traversePrompt.PromptData.Description,
                        ChatMessage.Alignment.Left
                    ));
                }

                // Move to the next parent
                traversePrompt = traversePrompt.PromptData.ParentPrompt;
            }
            else
            {
                // Different label, stop traversing
                break;
            }
        }

        // No need to reverse - we inserted at the beginning to maintain order
        return history;
    }
}

