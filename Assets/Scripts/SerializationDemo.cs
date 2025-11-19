using System;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [Header("Custom objects declared manually")]
    [SerializeField] private Modifier humanModifier;
    [SerializeField] private Modifier climaticModifier;
    [SerializeField] private Modifier aiModifier;


    public Action OnOver;

    public void Start()
    {
        OnOver += OnOverCallback;
        OnOver?.Invoke();
    }


    private void OnOverCallback()
    {
        Debug.Log("Over");
    }
}
[System.Serializable]
public class Gauge
{
    public float Value;
}
[System.Serializable]
public class Modifier
{
    public float AddedValue;
}