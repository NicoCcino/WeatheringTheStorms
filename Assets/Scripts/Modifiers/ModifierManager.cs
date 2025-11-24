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
}
