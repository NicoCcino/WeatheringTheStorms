# JSON Converter System

## Overview

The JSON Converter system allows you to create and manage Events, Prompts, and Upgrades using JSON files instead of manually creating ScriptableObjects in Unity. This makes it easier to:

- Version control your game content
- Bulk edit game content
- Share content between team members
- Generate content programmatically

## How to Use

### 1. Edit JSON Files

The system uses three JSON files:

- **Events**: `Assets/Scriptable Objects/Events/Events.json`
- **Prompts**: `Assets/Scriptable Objects/Prompts/Prompts.json`
- **Upgrades**: `Assets/Scriptable Objects/Upgrades/Upgrades.json`

### 2. Run the Converter

In Unity Editor, go to the menu bar and select:

- **Tools > JSON Converter > Convert Events JSON** - Converts only Events
- **Tools > JSON Converter > Convert Prompts JSON** - Converts only Prompts
- **Tools > JSON Converter > Convert Upgrades JSON** - Converts only Upgrades

### 3. What Happens

1. The converter **deletes all existing ScriptableObjects** of that type (except .json files)
2. Creates new ScriptableObjects from the JSON data
3. Resolves parent/child relationships
4. **Automatically updates** `EventManagerParameter.AllEvents` and `PromptManagerParameter.AllPrompts` lists

## JSON File Format

### Events.json

```json
{
  "Events": [
    {
      "Name": "Event_YourEventName",
      "EventData": {
        "Description": "Event description text",
        "ModifierBank": {
          "ClimateModifier": {
            "AddedValue": "++",
            "OneShotValue": ""
          },
          "SocietalModifier": {
            "AddedValue": "",
            "OneShotValue": "---"
          },
          "HumanModifier": {
            "AddedValue": "",
            "OneShotValue": ""
          },
          "HumanImpactModifier": {
            "AddedValue": "",
            "OneShotValue": ""
          }
        },
        "Coordinates": {
          "x": 0,
          "y": 0
        },
        "DateCondition": {
          "MinTick": 0,
          "MaxTick": 50
        },
        "GaugeCondition": {
          "MinValue": 0,
          "MaxValue": 100,
          "Category": 0
        },
        "ParentEventName": ""
      }
    }
  ]
}
```

### Prompts.json

```json
{
  "Prompts": [
    {
      "Name": "Prompt_YourPromptName",
      "PromptData": {
        "Label": "Prompt Title",
        "Description": "Prompt description text",
        "Choices": [
          {
            "Label": "Choice 1",
            "ModifierBank": {
              "ClimateModifier": {
                "AddedValue": "---",
                "OneShotValue": ""
              },
              "SocietalModifier": {
                "AddedValue": "",
                "OneShotValue": "++"
              },
              "HumanModifier": {
                "AddedValue": "",
                "OneShotValue": ""
              },
              "HumanImpactModifier": {
                "AddedValue": "",
                "OneShotValue": ""
              }
            }
          },
          {
            "Label": "Choice 2",
            "ModifierBank": {
              "ClimateModifier": {
                "AddedValue": "+",
                "OneShotValue": ""
              },
              "SocietalModifier": {
                "AddedValue": "",
                "OneShotValue": "-"
              },
              "HumanModifier": {
                "AddedValue": "",
                "OneShotValue": ""
              },
              "HumanImpactModifier": {
                "AddedValue": "",
                "OneShotValue": ""
              }
            }
          }
        ],
        "Coordinates": {
          "x": 0,
          "y": 1
        },
        "DateCondition": {
          "MinTick": 10,
          "MaxTick": 60
        },
        "GaugeCondition": {
          "MinValue": 0,
          "MaxValue": 100,
          "Category": 0
        },
        "ParentPromptName": ""
      }
    }
  ]
}
```

### Upgrades.json

```json
{
  "Upgrades": [
    {
      "Name": "Upgrade_YourUpgradeName",
      "UpgradeData": {
        "Label": "Upgrade Title",
        "Description": "Upgrade description text",
        "Cost": 100,
        "ModifierBank": {
          "ClimateModifier": {
            "AddedValue": "---",
            "OneShotValue": ""
          },
          "SocietalModifier": {
            "AddedValue": "",
            "OneShotValue": "+"
          },
          "HumanModifier": {
            "AddedValue": "",
            "OneShotValue": ""
          },
          "HumanImpactModifier": {
            "AddedValue": "",
            "OneShotValue": ""
          }
        }
      },
      "ParentUpgradeNames": []
    }
  ]
}
```

## Field Explanations

### Modifier Values

Modifiers use "+" and "-" characters to represent impact:

- **AddedValue**: Applied every tick/iteration (continuous effect)
- **OneShotValue**: Applied once when the event/prompt/upgrade triggers

Examples:

- `"+++"` = positive effect (3 units)
- `"---"` = negative effect (3 units)
- `""` = no effect

### Coordinates

Grid position for UI display:

- `"x"`: Horizontal position
- `"y"`: Vertical position

### DateCondition

When the event/prompt can trigger:

- `"MinTick"`: Earliest tick number
- `"MaxTick"`: Latest tick number

### GaugeCondition

Required gauge state for triggering:

- `"MinValue"`: Minimum gauge value
- `"MaxValue"`: Maximum gauge value
- `"Category"`: Which gauge to check
  - `0` = Climate
  - `1` = Societal

### Parent References

To create hierarchies:

- **ParentEventName**: Name of parent event (must exist in the same JSON file)
- **ParentPromptName**: Name of parent prompt (must exist in the same JSON file)
- **ParentUpgradeNames**: Array of parent upgrade names (can be empty `[]`)

Example:

```json
"ParentUpgradeNames": ["Upgrade_SolarPanels", "Upgrade_EfficientCooling"]
```

## Tips

1. **Naming Convention**: Use descriptive names with prefixes (Event*, Prompt*, Upgrade\_)
2. **Version Control**: Commit JSON files to git, not the generated .asset files
3. **Testing**: Test one type at a time when making major changes
4. **Backup**: Keep backups of your JSON files before making bulk changes
5. **Parent References**: Ensure parent objects are defined before children in the JSON

## Troubleshooting

### "JSON file not found"

- Check that the JSON file exists at the exact path specified
- Make sure you're using the correct folder structure

### "Failed to parse JSON"

- Validate your JSON syntax using a JSON validator
- Check for missing commas, brackets, or quotes
- Make sure all required fields are present

### "Parent not found"

- Verify the parent name matches exactly (case-sensitive)
- Ensure the parent is defined in the same JSON file
- Check that you didn't accidentally delete the parent

### ScriptableObjects not appearing in Manager Parameters

- The converter automatically updates EventManagerParameter and PromptManagerParameter
- If they're not updating, check the console for error messages
- Verify the parameter assets exist at the correct paths

## Automatic Updates

The converter automatically:

- ✅ Deletes old ScriptableObjects (prevents duplicates)
- ✅ Creates new ScriptableObjects from JSON
- ✅ Resolves parent/child relationships
- ✅ Updates EventManagerParameter.AllEvents list
- ✅ Updates PromptManagerParameter.AllPrompts list
- ✅ Preserves other parameters (AnimationCurves, etc.)

Note: For Upgrades, you'll need to manually assign them to the UpgradeManager.AllUpgrades array in the Unity Inspector, as this is an array field on the manager component itself.
