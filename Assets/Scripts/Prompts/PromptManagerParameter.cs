using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game/PromptManagerParameter", fileName = "PromptManagerParameter")]
public class PromptManagerParameter : ScriptableObject
{
    public List<Prompt> AllPrompts;
    public AnimationCurve PromptProbabilityOverTicks;

}

