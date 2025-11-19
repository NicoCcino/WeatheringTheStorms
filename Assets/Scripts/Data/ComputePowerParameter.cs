using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ComputePowerParameter", menuName = "Scriptable Objects/Game/ComputePowerParameter")]
public class ComputePowerParameter : ScriptableObject
{
    [Header("Compute Power Settings")]
    [Tooltip("Initial value of the compute power")]
    [SerializeField] public int StartValue = 0;

    [Tooltip("Base modifier of the compute power")]
    [SerializeField] public Modifier BaseModifier = new Modifier();
}
