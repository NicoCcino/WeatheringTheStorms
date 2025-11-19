using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GaugeParameter", menuName = "Scriptable Objects/Game/GaugeParameter")]
public class GaugeParameter : ScriptableObject
{
    [Header("Gauge Settings")]
    [Tooltip("Initial value of the gauge")]
    [SerializeField] public int StartValue = 0;

    [Tooltip("Base modifier of the gauge")]
    [SerializeField] public Modifier BaseModifier = new Modifier();

    [Tooltip("Min value of the gauge")]
    [SerializeField] public int Min = 0;

    [Tooltip("Max value of the gauge")]
    [SerializeField] public int Max = 100;
}
