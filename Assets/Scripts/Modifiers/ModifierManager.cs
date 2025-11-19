using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierManager
{
    private List<Modifier> modifiers = new List<Modifier>();

    public void Init(Modifier BaseModifier)
    {
        modifiers.Add(BaseModifier);
    }

    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
    }

    public float ComputeModifierValue()
    {
        float modifierValue = 0;
        foreach (var modifier in modifiers)
        {
            modifierValue += modifier.AddedValue;
        }
        return modifierValue;
    }
}
