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
    private HashSet<Prompt> TriggeredPrompts { get; set; } = new HashSet<Prompt>();
    public Action<Prompt> OnPromptTriggered;
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
        return TriggeredPrompts.Contains(promptToCheck.PromptData.ParentPrompt);
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
        //We add the prompt to the triggered prompts history
        TriggeredPrompts.Add(prompt);
        noPromptTickCounter = 0;
        Debug.Log($"Prompt {prompt.PromptData.Label} Triggered");
        OnPromptTriggered?.Invoke(prompt);
        Timeline.Instance.SetPauseSpeed();
        CurrentPrompt = prompt;
        prompt.OnSolved += OnCurrentPromptSolved;
        LogFileManager.Instance.LogUserAction("Prompt", prompt.PromptData.Label);
    }
    private void OnCurrentPromptSolved(Choice choice)
    {
        if (CurrentPrompt == null) return;

        Timeline.Instance.SetPlaySpeed();
        CurrentPrompt.OnSolved -= OnCurrentPromptSolved;
    }
}

