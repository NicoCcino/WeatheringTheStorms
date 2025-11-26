using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierManager
{
    public float addedValue = 0.0f;

    public Action<Modifier> OnModifierAdded;

    public void AddModifier(Modifier modifier)
    {
        addedValue += modifier.AddedValue;
        OnModifierAdded?.Invoke(modifier);
    }
}
