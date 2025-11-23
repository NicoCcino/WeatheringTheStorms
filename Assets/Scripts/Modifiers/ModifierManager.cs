using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierManager
{
    private List<Modifier> modifiers = new List<Modifier>();
    public float modifierScale;

    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
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

    public float ComputeOneShotValue(Modifier modifier)
    {
        return modifier.OneShotValue * modifierScale;
    }
}
