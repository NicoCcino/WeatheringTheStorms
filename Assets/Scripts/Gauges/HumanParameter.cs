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

    [Tooltip("Human population impact on gauges, the bigger the greater the impact")]
    public float HumanImpact;

    [Tooltip("Scale of the human modifiers")]
    [SerializeField] public float HumanModifierScale = 0.01f;
}
