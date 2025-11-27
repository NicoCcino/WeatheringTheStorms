using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Utility to auto-balance prompt choices by offsetting their modifiers to reach target expected values.
/// This ensures that prompts are balanced and don't systematically favor one gauge over another.
/// </summary>
public class PromptBalancer : MonoBehaviour
{
    [Tooltip("The PromptManagerParameter containing all prompts to balance")]
    public PromptManagerParameter promptManagerParameter;

    [Tooltip("The PromptBalancerParameter containing the target values")]
    public PromptBalancerParameter promptBalancerParameter;

    // Report data for Inspector display
    [System.NonSerialized] public string reportSummary = "";
    [System.NonSerialized] public List<string> reportDetails = new List<string>();
    [System.NonSerialized] public bool hasReport = false;

    private enum ModifierType { Climate, Societal, Trust }

    [ContextMenu("Balance All Prompts")]
    public void BalanceAllPrompts()
    {
#if UNITY_EDITOR
        reportDetails.Clear();
        int totalModified = 0;

        foreach (ModifierType type in Enum.GetValues(typeof(ModifierType)))
        {
            float meanBefore = CalculateMean(type);
            float amplitudeBefore = CalculateAmplitude(type);
            float targetMean = GetTargetMean(type);
            float targetAmplitude = GetTargetAmplitude(type);

            // 1. Scale to target amplitude first
            float scale = targetAmplitude / amplitudeBefore;
            ApplyScale(type, scale);

            // 2. Then shift to target mean (after scaling)
            float meanAfterScale = CalculateMean(type);
            float offset = targetMean - meanAfterScale;
            int modifiedCount = ApplyOffset(type, offset);

            // Calculate final values for report
            float meanAfter = CalculateMean(type);
            float amplitudeAfter = CalculateAmplitude(type);
            
            totalModified += modifiedCount;
            reportDetails.Add($"── {type} ──");
            reportDetails.Add($"  Mean: {meanBefore:F2} → {meanAfter:F2} (target: {targetMean})");
            reportDetails.Add($"  Amplitude: {amplitudeBefore:F2} → {amplitudeAfter:F2} (target: {targetAmplitude})");
            reportDetails.Add($"  Values modified: {modifiedCount}");
        }

        reportSummary = $"Balanced {totalModified} modifier values across {promptManagerParameter.AllPrompts.Count} prompts.";
        hasReport = true;
        
        AssetDatabase.SaveAssets();
#endif
    }

    private float CalculateMean(ModifierType type)
    {
        float sum = 0;
        int count = 0;

        foreach (var prompt in promptManagerParameter.AllPrompts)
        {
            foreach (var choice in prompt.PromptData.Choices)
            {
                bool isThereAValue = false;
                Modifier currentModifier = GetModifierValue(choice.ModifierBank, type);
                if (currentModifier.OneShotValue != 0.0f)
                {
                    sum += currentModifier.OneShotValue;
                    isThereAValue = true;
                }
                if (choice.PlannedAction != null && choice.PlannedAction.PlannedEvent != null)
                {
                    // We have a linked event, so we need to add the event modifier values to the sum
                    currentModifier = GetModifierValue(choice.PlannedAction.PlannedEvent.EventData.ModifierBank, type);
                    if (currentModifier.AddedValue != 0.0f)
                    {
                        sum += currentModifier.AddedValue * choice.PlannedAction.TicksDelay;
                    }
                    isThereAValue = true;
                }
                if (isThereAValue) count++;
            }
        }

        return count > 0 ? sum / count : 0;
    }

    private float CalculateAmplitude(ModifierType type)
    {
        float max = 0;
        float min = 0;
        foreach (var prompt in promptManagerParameter.AllPrompts)
        {
            foreach (var choice in prompt.PromptData.Choices)
            {
                float TotalModifierValue = 0;
                Modifier currentModifier = GetModifierValue(choice.ModifierBank, type);
                if (currentModifier.OneShotValue != 0)
                {
                    TotalModifierValue += currentModifier.OneShotValue;
                }
                if (choice.PlannedAction != null && choice.PlannedAction.PlannedEvent != null)
                {
                    currentModifier = GetModifierValue(choice.PlannedAction.PlannedEvent.EventData.ModifierBank, type);
                    if (currentModifier.AddedValue != 0)
                    {
                        TotalModifierValue += currentModifier.AddedValue * choice.PlannedAction.TicksDelay;
                    }
                }
                if (TotalModifierValue != 0)
                {
                    max = Mathf.Max(max, TotalModifierValue);
                    min = Mathf.Min(min, TotalModifierValue);
                }
            }
        }
        return max - min;
    }

    private int ApplyOffset(ModifierType type, float offset)
    {
#if UNITY_EDITOR
        int modified = 0;

        foreach (var prompt in promptManagerParameter.AllPrompts)
        {
            foreach (var choice in prompt.PromptData.Choices)
            {
                Modifier currentModifier = GetModifierValue(choice.ModifierBank, type);
                if (currentModifier.OneShotValue != 0)
                {
                    currentModifier.OneShotValue += offset;
                    modified++;
                }
                if (choice.PlannedAction != null && choice.PlannedAction.PlannedEvent != null && choice.PlannedAction.TicksDelay > 0)
                {
                    currentModifier = GetModifierValue(choice.PlannedAction.PlannedEvent.EventData.ModifierBank, type);
                    if (currentModifier.AddedValue != 0)
                    {
                        currentModifier.AddedValue += offset / choice.PlannedAction.TicksDelay;
                    }
                }
            }
            EditorUtility.SetDirty(prompt);
        }

        return modified;
#else
        return 0;
#endif
    }

    private int ApplyScale(ModifierType type, float scale)
    {
        int modified = 0;
        foreach (var prompt in promptManagerParameter.AllPrompts)
        {
            foreach (var choice in prompt.PromptData.Choices)
            {
                Modifier currentModifier = GetModifierValue(choice.ModifierBank, type);
                if (currentModifier.OneShotValue != 0)
                {
                    currentModifier.OneShotValue *= scale;
                    modified++;
                }
                if (choice.PlannedAction != null && choice.PlannedAction.PlannedEvent != null)
                {
                    currentModifier = GetModifierValue(choice.PlannedAction.PlannedEvent.EventData.ModifierBank, type);
                    if (currentModifier.AddedValue != 0)
                    {
                        currentModifier.AddedValue *= scale;
                    }
                }
            }
        }
        return modified;
    }

    private float GetTargetMean(ModifierType type)
    {
        return type switch
        {
            ModifierType.Climate => promptBalancerParameter.gameBalancingParameter.targetGaugeEsperanceValue,
            ModifierType.Societal => promptBalancerParameter.gameBalancingParameter.targetGaugeEsperanceValue,
            ModifierType.Trust => promptBalancerParameter.targetTrustEsperanceValue,
            _ => 0
        };
    }

    private float GetTargetAmplitude(ModifierType type)
    {
        return type switch
        {
            ModifierType.Climate => promptBalancerParameter.gameBalancingParameter.targetGaugeMaxAmplitude,
            ModifierType.Societal => promptBalancerParameter.gameBalancingParameter.targetGaugeMaxAmplitude,
            ModifierType.Trust => promptBalancerParameter.targetTrustMaxAmplitude,
            _ => 0
        };
    }
    private Modifier GetModifierValue(ModifierBank bank, ModifierType type)
    {
        return type switch
        {
            ModifierType.Climate => bank.ClimateModifier,
            ModifierType.Societal => bank.SocietalModifier,
            ModifierType.Trust => bank.TrustModifier,
            _ => null
        };
    }
}
