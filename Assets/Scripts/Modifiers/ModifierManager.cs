using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ModifierManager
{
    private List<Modifier> modifiers = new List<Modifier>();
    public float modifierScale;

    public void AddModifier(Modifier modifier)
    {
        if (!string.IsNullOrEmpty(modifier.AddedValue))
        {
            string addedValueTrimmed = modifier.AddedValue.Trim();
            if (!(addedValueTrimmed.All(c => c == '+' || char.IsWhiteSpace(c)) ||
                  addedValueTrimmed.All(c => c == '-' || char.IsWhiteSpace(c))))
            {
                Debug.LogWarning("Modifier.AddedValue should contain only '+' or only '-' characters.");
                return;
            }
        }
        if (!string.IsNullOrEmpty(modifier.OneShotValue))
        {
            string oneShotValueTrimmed = modifier.OneShotValue.Trim();
            if (!(oneShotValueTrimmed.All(c => c == '+' || char.IsWhiteSpace(c)) ||
                  oneShotValueTrimmed.All(c => c == '-' || char.IsWhiteSpace(c))))
            {
                Debug.LogWarning("Modifier.OneShotValue should contain only '+' or only '-' characters.");
                return;
            }
        }
        modifiers.Add(modifier);
    }

    public float ComputeModifierValue()
    {
        float modifierValue = 0;
        foreach (var modifier in modifiers)
        {
            if (!string.IsNullOrEmpty(modifier.AddedValue))
            {
                foreach (char c in modifier.AddedValue)
                {
                    if (c == '+')
                        modifierValue += modifierScale;
                    else if (c == '-')
                        modifierValue -= modifierScale;
                    // Ignore whitespace and other unexpected characters
                }
            }
        }
        return modifierValue;
    }

    public float ComputeOneShotValue(Modifier modifier)
    {
        float oneShotValue = 0;
        if (!string.IsNullOrEmpty(modifier.OneShotValue))
        {
            foreach (char c in modifier.OneShotValue)
            {
                if (c == '+')
                    oneShotValue += modifierScale;
                else if (c == '-')
                    oneShotValue -= modifierScale;
                // Ignore whitespace and other unexpected characters
            }
        }
        return oneShotValue;
    }
}
