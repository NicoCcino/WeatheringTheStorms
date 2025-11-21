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

    [Tooltip("The higher the value, the steeper the exponetial curve is on human effect on gauges")]
    public float HumanPopulationImpactPower;

    [Tooltip("Factor of scale of the human population impact, the bigger the greater the impact")]
    public float HumanPopulationImpactScale;

    [Tooltip("If HumanCount > this value, the impact exponential starts to be > 1 , if HumanCount< this value, the impact exponential starts to be <1")]
    public float TuningValue;
}
