using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HumanParameter", menuName = "Scriptable Objects/Game/HumanParameter")]
public class HumanParameter : ScriptableObject
{
    [Header("Human Settings")]
    [Tooltip("Initial value of the number of humans")]
    [SerializeField] public int StartValue = 0;

    [Tooltip("Base modifier of the humans")]
    [SerializeField] public Modifier BaseModifier = new Modifier();
}
