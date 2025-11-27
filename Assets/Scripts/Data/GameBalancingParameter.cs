using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameBalancingParameter", menuName = "Scriptable Objects/Game/GameBalancingParameter")]
public class GameBalancingParameter : ScriptableObject
{
    [Header("Compute Power Settings base frequency")]
    [SerializeField] public float SpawnFrequency;

    [Header("Prompt Balancer Settings")]
    public float targetGaugeEsperanceValue = 0f;
    public float targetGaugeMaxAmplitude = 10f;

   
}
