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

    [Tooltip("Scale of the modifiers")]
    [SerializeField] public float ModifierScale = 0.01f;

    [Tooltip("If true, the gauge will be impacted by the human population")]
    [SerializeField] public bool LinkedToHumans = true;

    public float DecayingPerTick = -1.1f;
}
