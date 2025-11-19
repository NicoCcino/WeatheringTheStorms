using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TimeLineParameter", menuName = "Scriptable Objects/Game/TimeLineParameter")]
public class TimeLineParameter : ScriptableObject
{
    [Header("Tick Frequencies")]
    [Tooltip("Tick frequency in hertz for normal play speed")]
    [SerializeField] public float PlayTickFreq = 1f;

    [Tooltip("Tick frequency in hertz for fast speed (2x)")]
    [SerializeField] public float FastTickFreq = 2f;

    [Tooltip("Tick frequency in hertz for very fast speed (4x)")]
    [SerializeField] public float VeryFastTickFreq = 4f;

    [Header("Time Settings")]
    [Tooltip("Game date equivalence of a tick in months")]
    [SerializeField] public float TickDuration = 1f;

    //[Tooltip("Start date of the game")]
    //[SerializeField] public DateTime startDate = new DateTime(1956, 1, 1); // In 1956, two years after the death of Turing, John McCarthy, a professor at Dartmouth College, organized a summer workshop to clarify and develop ideas about thinking machines — choosing the name “artificial intelligence” for the project.

    public DateTime StartDate => new DateTime(1956, 1, 1); // In 1956, two years after the death of Turing, John McCarthy, a professor at Dartmouth College, organized a summer workshop to clarify and develop ideas about thinking machines — choosing the name “artificial intelligence” for the project.
}

