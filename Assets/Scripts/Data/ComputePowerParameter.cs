using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ComputePowerParameter", menuName = "Scriptable Objects/Game/ComputePowerParameter")]
public class ComputePowerParameter : ScriptableObject
{
    [Header("Compute Power Settings")]
    [Tooltip("Initial value of the compute power")]
    [SerializeField] public int StartValue = 0;

    [Tooltip("Growth rate of the compute power per tick")]
    [SerializeField] public float BaseModifier = 0.01f;


    [Header("Clickable computer power settings")]
    [SerializeField] public int ComputePowerClickableGain;
    [SerializeField] public float SpawnFrequency;
    [SerializeField] public float TrustSpawnFrequencyImpact = 0.01f;
}
