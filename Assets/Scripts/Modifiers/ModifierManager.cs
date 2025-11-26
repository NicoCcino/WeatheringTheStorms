using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierManager
{
    public List<Modifier> modifiers = new List<Modifier>();
    public float modifierScale;

    public Action<Modifier> OnModifierAdded;
    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        OnModifierAdded?.Invoke(modifier);
    }

    public float ComputeModifierValue()
    {
        float modifierValue = 0;
        foreach (var modifier in modifiers)
        {
            modifierValue += modifier.AddedValue * modifierScale;
        }
        return modifierValue;
    }
}
