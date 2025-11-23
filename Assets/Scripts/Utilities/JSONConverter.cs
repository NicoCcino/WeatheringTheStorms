using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable JSON classes for converting JSON data to Unity ScriptableObjects
/// </summary>
namespace JSONData
{
    [Serializable]
    public class ModifierJSON
    {
        public int AddedValue = 0;
        public int OneShotValue = 0;

        public Modifier ToModifier()
        {
            return new Modifier
            {
                AddedValue = AddedValue,
                OneShotValue = OneShotValue
            };
        }
    }

    [Serializable]
    public class ModifierBankJSON
    {
        public ModifierJSON ClimateModifier;
        public ModifierJSON SocietalModifier;
        public ModifierJSON HumanModifier;
        public ModifierJSON HumanImpactModifier;

        public ModifierBank ToModifierBank()
        {
            return new ModifierBank
            {
                ClimateModifier = ClimateModifier?.ToModifier() ?? new Modifier(),
                SocietalModifier = SocietalModifier?.ToModifier() ?? new Modifier(),
                HumanModifier = HumanModifier?.ToModifier() ?? new Modifier(),
                HumanImpactModifier = HumanImpactModifier?.ToModifier() ?? new Modifier()
            };
        }
    }

    [Serializable]
    public class DateConditionJSON
    {
        public int MinTick;
        public int MaxTick;

        public DateCondition ToDateCondition()
        {
            return new DateCondition(MinTick, MaxTick);
        }
    }

    [Serializable]
    public class GaugeConditionJSON
    {
        public int MinValue;
        public int MaxValue;
        public int Category; // 0 = Climate, 1 = Societal

        public GaugeCondition ToGaugeCondition()
        {
            return new GaugeCondition(MinValue, MaxValue, (PromptData.PromptCategory)Category);
        }
    }

    [Serializable]
    public class Vector2IntJSON
    {
        public int x;
        public int y;

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }
    }

    [Serializable]
    public class ChoiceJSON
    {
        public string Label;
        public ModifierBankJSON ModifierBank;

        public Choice ToChoice()
        {
            return new Choice
            {
                Label = Label,
                ModifierBank = ModifierBank?.ToModifierBank() ?? new ModifierBank()
            };
        }
    }

    [Serializable]
    public class PlannedActionJSON
    {
        public uint TicksDelay;
        public string PlannedPromptName; // Reference by name, will be resolved later
        public string PlannedEventName; // Reference by name, will be resolved later
    }

    [Serializable]
    public class EventDataJSON
    {
        public string Description;
        public int DurationInTicks;
        public string Icon;
        public ModifierBankJSON ModifierBank;
        public Vector2IntJSON Coordinates;
        public DateConditionJSON DateCondition;
        public GaugeConditionJSON GaugeCondition;
        public string ParentEventName; // Reference by name, will be resolved later
        public PlannedActionJSON PlannedAction; // Optional planned action
    }

    [Serializable]
    public class EventJSON
    {
        public string Name; // Used as the asset name
        public EventDataJSON EventData;
    }

    [Serializable]
    public class EventListJSON
    {
        public List<EventJSON> Events;
    }

    [Serializable]
    public class PromptDataJSON
    {
        public string Label;
        public string Description;
        public List<ChoiceJSON> Choices;
        public Vector2IntJSON Coordinates; // In JSON but will be set on Prompt, not PromptData
        public DateConditionJSON DateCondition;
        public GaugeConditionJSON GaugeCondition;
        public string ParentPromptName; // Reference by name, will be resolved later
        public PlannedActionJSON PlannedAction; // Optional planned action
    }

    [Serializable]
    public class PromptJSON
    {
        public string Name; // Used as the asset name
        public PromptDataJSON PromptData;
    }

    [Serializable]
    public class PromptListJSON
    {
        public List<PromptJSON> Prompts;
    }

    [Serializable]
    public class UpgradeDataJSON
    {
        public string Label;
        public string Description;
        public int Cost;
        public ModifierBankJSON ModifierBank;
    }

    [Serializable]
    public class UpgradeJSON
    {
        public string Name; // Used as the asset name
        public UpgradeDataJSON UpgradeData;
        public List<string> ParentUpgradeNames; // References by name, will be resolved later
    }

    [Serializable]
    public class UpgradeListJSON
    {
        public List<UpgradeJSON> Upgrades;
    }
}

