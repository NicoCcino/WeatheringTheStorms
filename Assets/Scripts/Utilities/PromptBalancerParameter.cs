using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PromptBalancerParameter", menuName = "Scriptable Objects/Game/PromptBalancerParameter")]
public class PromptBalancerParameter : ScriptableObject
{
    [Header("Prompt Balancer Settings")]
    public GameBalancingParameter gameBalancingParameter;
    public float targetTrustEsperanceValue = 0f;
    public float targetTrustMaxAmplitude = 10f;
}
