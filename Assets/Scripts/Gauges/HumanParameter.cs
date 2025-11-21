using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HumanParameter", menuName = "Scriptable Objects/Game/HumanParameter")]
public class HumanParameter : ScriptableObject
{
    [Header("Human Settings")]
    [Tooltip("Initial value of the number of humans")]
    [SerializeField] public float StartValue = 8160000000;

    [Tooltip("population growth % per year")]
    [SerializeField] public float PopulationGrowthPerYear = 0.01f;

    [Tooltip("Gauge impact per human")]
    [SerializeField] public float GaugeImpactPerHuman = -2e-11f; // Calculated to make the gauge reach 0 in less than 5 minutes on Play speed (less than 25 game years)
}
