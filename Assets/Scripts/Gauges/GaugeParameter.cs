using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GaugeParameter", menuName = "Scriptable Objects/Game/GaugeParameter")]
public class GaugeParameter : ScriptableObject
{
    [Header("Gauge Settings")]
    [Tooltip("Initial value of the gauge")]
    [SerializeField] public int StartValue = 0;

    [Tooltip("Min value of the gauge")]
    [SerializeField] public int Min = 0;

    [Tooltip("Max value of the gauge")]
    [SerializeField] public int Max = 100;
}
