# Quick Start Guide - JSON Converter

## 🚀 Quick Steps

1. **Edit the JSON files:**

   - Events: `Events/Events.json`
   - Prompts: `Prompts/Prompts.json`
   - Upgrades: `Upgrades/Upgrades.json`

2. **In Unity, go to:**

   ```
   Tools > JSON Converter > Convert Events JSON
   Tools > JSON Converter > Convert Prompts JSON
   Tools > JSON Converter > Convert Upgrades JSON
   ```

   (Run each converter for the types you want to update)

3. **Done!** Your ScriptableObjects are created and manager parameters are updated.

## 📝 Modifier Quick Reference

### Format

```json
"ModifierBank": {
  "ClimateModifier": {
    "AddedValue": "+++",      // Continuous effect per tick
    "OneShotValue": "---"     // One-time effect
  },
  "SocietalModifier": {...},
  "HumanModifier": {...},
  "HumanImpactModifier": {...}
}
```

### Values

- `"+++"` = +3 positive effect
- `"---"` = -3 negative effect
- `""` = no effect
- Use more + or - for stronger effects

## 🎯 Category Reference

For `GaugeCondition.Category`:

- `0` = Climate
- `1` = Societal

## 🔗 Parent References

### Events & Prompts (single parent)

```json
"ParentEventName": "Event_DataCenterFire"
"ParentPromptName": "Prompt_DataPrivacy"
```

### Upgrades (multiple parents)

```json
"ParentUpgradeNames": ["Upgrade_Solar", "Upgrade_Cooling"]
// or empty for root upgrades:
"ParentUpgradeNames": []
```

## ⚠️ Important

- The converter **deletes old ScriptableObjects** - your changes in Unity Inspector will be lost
- Always edit the **JSON files**, not the ScriptableObjects directly
- Parent names must match exactly (case-sensitive)
- Parent objects must exist in the same JSON file

## ✅ What Gets Auto-Updated

- ✅ EventManagerParameter.AllEvents
- ✅ PromptManagerParameter.AllPrompts
- ⚠️ UpgradeManager.AllUpgrades (manual assignment needed)

## 📖 Full Documentation

See `Assets/Scripts/Utilities/JSONConverter_README.md` for complete documentation.
