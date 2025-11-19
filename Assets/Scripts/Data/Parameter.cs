using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Parameter", menuName = "Game/Parameter")]
public class Parameter : ScriptableObject
{
    [Header("Tick Frequencies")]
    [Tooltip("Tick frequency in hertz for normal play speed")]
    [SerializeField] private float playTickFreq = 1f;
    
    [Tooltip("Tick frequency in hertz for fast speed (2x)")]
    [SerializeField] private float fastTickFreq = 2f;
    
    [Tooltip("Tick frequency in hertz for very fast speed (4x)")]
    [SerializeField] private float veryFastTickFreq = 4f;
    
    [Header("Time Settings")]
    [Tooltip("Game date equivalence of a tick in months")]
    [SerializeField] private float tickDuration = 1f;

    //[Tooltip("Start date of the game")]
    //[SerializeField] private DateTime startDate = new DateTime(1956, 1, 1); // In 1956, two years after the death of Turing, John McCarthy, a professor at Dartmouth College, organized a summer workshop to clarify and develop ideas about thinking machines — choosing the name “artificial intelligence” for the project.
    
    // Public properties for accessing the values
    public float PlayTickFreq => playTickFreq;
    public float FastTickFreq => fastTickFreq;
    public float VeryFastTickFreq => veryFastTickFreq;
    public float TickDuration => tickDuration;
    public DateTime StartDate => new DateTime(1956, 1, 1); // In 1956, two years after the death of Turing, John McCarthy, a professor at Dartmouth College, organized a summer workshop to clarify and develop ideas about thinking machines — choosing the name “artificial intelligence” for the project.
}

