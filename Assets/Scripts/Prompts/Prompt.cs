using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/Prompt", fileName = "Prompt")]
public class Prompt : ScriptableObject
{
    public PromptData PromptData;
    public Action<Choice> OnSolved;
    public void Solve(Choice choice)
    {
        OnSolved?.Invoke(choice);

        GaugeManager.Instance.ApplyModifierBank(choice.ModifierBank);
    }
}

