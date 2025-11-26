#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using JSONData;

public class JSONConverterEditor : EditorWindow
{
    private const string EVENTS_JSON_PATH = "Assets/Scriptable Objects/Events/Events.json";
    private const string PROMPTS_JSON_PATH = "Assets/Scriptable Objects/Prompts/Prompts.json";
    private const string UPGRADES_JSON_PATH = "Assets/Scriptable Objects/Upgrades/Upgrades.json";

    private const string EVENTS_FOLDER = "Assets/Scriptable Objects/Events";
    private const string PROMPTS_FOLDER = "Assets/Scriptable Objects/Prompts";
    private const string UPGRADES_FOLDER = "Assets/Scriptable Objects/Upgrades";
    private const string ICONS_FOLDER = "Assets/Images/UI/Icons";

    private const string EVENT_MANAGER_PARAM_PATH = "Assets/Scriptable Objects/EventManagerParameter.asset";
    private const string PROMPT_MANAGER_PARAM_PATH = "Assets/Scriptable Objects/PromptManagerParameter.asset";

    [MenuItem("Tools/JSON Converter/Convert Events JSON")]
    public static void ConvertEventsJSONMenu()
    {
        if (!File.Exists(EVENTS_JSON_PATH))
        {
            EditorUtility.DisplayDialog("Error", $"Events JSON file not found at: {EVENTS_JSON_PATH}", "OK");
            return;
        }

        if (ConvertEventsJSON())
        {
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Events converted successfully!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to convert events. Check the console for details.", "OK");
        }
    }

    [MenuItem("Tools/JSON Converter/Convert Prompts JSON")]
    public static void ConvertPromptsJSONMenu()
    {
        if (!File.Exists(PROMPTS_JSON_PATH))
        {
            EditorUtility.DisplayDialog("Error", $"Prompts JSON file not found at: {PROMPTS_JSON_PATH}", "OK");
            return;
        }

        if (ConvertPromptsJSON())
        {
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Prompts converted successfully!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to convert prompts. Check the console for details.", "OK");
        }
    }

    [MenuItem("Tools/JSON Converter/Convert Upgrades JSON")]
    public static void ConvertUpgradesJSONMenu()
    {
        if (!File.Exists(UPGRADES_JSON_PATH))
        {
            EditorUtility.DisplayDialog("Error", $"Upgrades JSON file not found at: {UPGRADES_JSON_PATH}", "OK");
            return;
        }

        if (ConvertUpgradesJSON())
        {
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Upgrades converted successfully!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to convert upgrades. Check the console for details.", "OK");
        }
    }

    private static bool ConvertEventsJSON()
    {
        try
        {
            string jsonContent = File.ReadAllText(EVENTS_JSON_PATH);
            string jsonWithoutComments = StripComments(jsonContent);
            EventListJSON eventList = JsonUtility.FromJson<EventListJSON>(jsonWithoutComments);

            if (eventList == null || eventList.Events == null || eventList.Events.Count == 0)
            {
                Debug.LogError("Failed to parse Events JSON or the list is empty.");
                return false;
            }

            // Delete existing Event ScriptableObjects (except .json and .meta files)
            DeleteExistingScriptableObjects(EVENTS_FOLDER, "t:Event");

            // Create dictionary for resolving parent references
            Dictionary<string, Event> createdEvents = new Dictionary<string, Event>();

            // First pass: Create all events
            foreach (var eventJSON in eventList.Events)
            {
                Event eventAsset = ScriptableObject.CreateInstance<Event>();
                
                // Use reflection to set private properties
                var eventDataField = typeof(Event).GetField("EventData");
                EventData eventData = new EventData();
                
                SetPrivateProperty(eventData, "Description", eventJSON.EventData.Description);
                SetPrivateProperty(eventData, "DurationInTicks", eventJSON.EventData.DurationInTicks);
                
                // Load icon sprite from file name
                Sprite iconSprite = LoadIconSprite(eventJSON.EventData.Icon);
                SetPrivateProperty(eventData, "Icon", iconSprite);
                
                SetPrivateProperty(eventData, "ModifierBank", eventJSON.EventData.ModifierBank?.ToModifierBank() ?? new ModifierBank());
                SetPrivateProperty(eventData, "Coordinates", eventJSON.EventData.Coordinates?.ToVector2Int() ?? Vector2Int.zero);
                SetPrivateProperty(eventData, "DateCondition", eventJSON.EventData.DateCondition?.ToDateCondition());
                SetPrivateProperty(eventData, "GaugeCondition", eventJSON.EventData.GaugeCondition?.ToGaugeCondition());
                
                eventDataField.SetValue(eventAsset, eventData);

                string assetPath = $"{EVENTS_FOLDER}/{eventJSON.Name}.asset";
                AssetDatabase.CreateAsset(eventAsset, assetPath);
                createdEvents[eventJSON.Name] = eventAsset;
                
                Debug.Log($"Created Event: {eventJSON.Name}");
            }

            // Second pass: Resolve parent event references
            for (int i = 0; i < eventList.Events.Count; i++)
            {
                var eventJSON = eventList.Events[i];
                if (!string.IsNullOrEmpty(eventJSON.EventData.ParentEventName))
                {
                    if (createdEvents.TryGetValue(eventJSON.EventData.ParentEventName, out Event parentEvent))
                    {
                        Event currentEvent = createdEvents[eventJSON.Name];
                        var eventDataField = typeof(Event).GetField("EventData");
                        EventData eventData = (EventData)eventDataField.GetValue(currentEvent);
                        SetPrivateProperty(eventData, "ParentEvent", parentEvent);
                        EditorUtility.SetDirty(currentEvent);
                    }
                    else
                    {
                        Debug.LogWarning($"Parent event '{eventJSON.EventData.ParentEventName}' not found for event '{eventJSON.Name}'");
                    }
                }
            }

            AssetDatabase.SaveAssets();

            // Third pass: Resolve PlannedAction references (need all Prompts to be loaded too)
            ResolveEventPlannedActions(eventList, createdEvents);

            // Update EventManagerParameter
            UpdateEventManagerParameter(createdEvents.Values.ToList());

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting Events JSON: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private static bool ConvertPromptsJSON()
    {
        try
        {
            string jsonContent = File.ReadAllText(PROMPTS_JSON_PATH);
            string jsonWithoutComments = StripComments(jsonContent);
            PromptListJSON promptList = JsonUtility.FromJson<PromptListJSON>(jsonWithoutComments);

            if (promptList == null || promptList.Prompts == null || promptList.Prompts.Count == 0)
            {
                Debug.LogError("Failed to parse Prompts JSON or the list is empty.");
                return false;
            }

            // Delete existing Prompt ScriptableObjects
            DeleteExistingScriptableObjects(PROMPTS_FOLDER, "t:Prompt");

            // Create dictionary for resolving parent references
            Dictionary<string, Prompt> createdPrompts = new Dictionary<string, Prompt>();

            // First pass: Create all prompts
            foreach (var promptJSON in promptList.Prompts)
            {
                Prompt promptAsset = ScriptableObject.CreateInstance<Prompt>();
                
                var promptDataField = typeof(Prompt).GetField("PromptData");
                PromptData promptData = new PromptData();
                
                SetPrivateProperty(promptData, "Label", promptJSON.PromptData.Label);
                SetPrivateProperty(promptData, "Description", promptJSON.PromptData.Description);
                
                // Convert choices
                Choice[] choices = promptJSON.PromptData.Choices?.Select(c => c.ToChoice()).ToArray() ?? new Choice[0];
                SetPrivateProperty(promptData, "Choices", choices);
                
                SetPrivateProperty(promptData, "DateCondition", promptJSON.PromptData.DateCondition?.ToDateCondition());
                SetPrivateProperty(promptData, "GaugeCondition", promptJSON.PromptData.GaugeCondition?.ToGaugeCondition());
                
                promptDataField.SetValue(promptAsset, promptData);
                
                // Set Coordinates on the Prompt object itself (not PromptData)
                // Note: Coordinates are in the JSON under PromptData, but we set them on Prompt
                promptAsset.Coordinates = promptJSON.PromptData.Coordinates?.ToVector2Int() ?? Vector2Int.zero;

                string assetPath = $"{PROMPTS_FOLDER}/{promptJSON.Name}.asset";
                AssetDatabase.CreateAsset(promptAsset, assetPath);
                createdPrompts[promptJSON.Name] = promptAsset;
                
                Debug.Log($"Created Prompt: {promptJSON.Name}");
            }

            // Second pass: Resolve parent prompt references
            for (int i = 0; i < promptList.Prompts.Count; i++)
            {
                var promptJSON = promptList.Prompts[i];
                if (!string.IsNullOrEmpty(promptJSON.PromptData.ParentPromptName))
                {
                    if (createdPrompts.TryGetValue(promptJSON.PromptData.ParentPromptName, out Prompt parentPrompt))
                    {
                        Prompt currentPrompt = createdPrompts[promptJSON.Name];
                        var promptDataField = typeof(Prompt).GetField("PromptData");
                        PromptData promptData = (PromptData)promptDataField.GetValue(currentPrompt);
                        SetPrivateProperty(promptData, "ParentPrompt", parentPrompt);
                        EditorUtility.SetDirty(currentPrompt);
                    }
                    else
                    {
                        Debug.LogWarning($"Parent prompt '{promptJSON.PromptData.ParentPromptName}' not found for prompt '{promptJSON.Name}'");
                    }
                }
            }

            AssetDatabase.SaveAssets();

            // Third pass: Resolve PlannedAction references (need all Events to be loaded too)
            ResolvePromptPlannedActions(promptList, createdPrompts);

            // Update PromptManagerParameter
            UpdatePromptManagerParameter(createdPrompts.Values.ToList());

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting Prompts JSON: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private static bool ConvertUpgradesJSON()
    {
        try
        {
            string jsonContent = File.ReadAllText(UPGRADES_JSON_PATH);
            string jsonWithoutComments = StripComments(jsonContent);
            UpgradeListJSON upgradeList = JsonUtility.FromJson<UpgradeListJSON>(jsonWithoutComments);

            if (upgradeList == null || upgradeList.Upgrades == null || upgradeList.Upgrades.Count == 0)
            {
                Debug.LogError("Failed to parse Upgrades JSON or the list is empty.");
                return false;
            }

            // Delete existing Upgrade ScriptableObjects recursively
            DeleteExistingScriptableObjects(UPGRADES_FOLDER, "t:Upgrade");

            // Create dictionary for resolving parent references
            Dictionary<string, Upgrade> createdUpgrades = new Dictionary<string, Upgrade>();

            // First pass: Create all upgrades
            foreach (var upgradeJSON in upgradeList.Upgrades)
            {
                Upgrade upgradeAsset = ScriptableObject.CreateInstance<Upgrade>();
                
                var upgradeDataField = typeof(Upgrade).GetField("UpgradeData");
                UpgradeData upgradeData = new UpgradeData();
                
                SetPrivateProperty(upgradeData, "Label", upgradeJSON.UpgradeData.Label);
                SetPrivateProperty(upgradeData, "Description", upgradeJSON.UpgradeData.Description);
                SetPrivateProperty(upgradeData, "Cost", upgradeJSON.UpgradeData.Cost);
                SetPrivateProperty(upgradeData, "ModifierBank", upgradeJSON.UpgradeData.ModifierBank?.ToModifierBank() ?? new ModifierBank());
                
                upgradeDataField.SetValue(upgradeAsset, upgradeData);

                string assetPath = $"{UPGRADES_FOLDER}/{upgradeJSON.Name}.asset";
                AssetDatabase.CreateAsset(upgradeAsset, assetPath);
                createdUpgrades[upgradeJSON.Name] = upgradeAsset;
                
                Debug.Log($"Created Upgrade: {upgradeJSON.Name}");
            }

            // Second pass: Resolve parent upgrade references
            for (int i = 0; i < upgradeList.Upgrades.Count; i++)
            {
                var upgradeJSON = upgradeList.Upgrades[i];
                if (upgradeJSON.ParentUpgradeNames != null && upgradeJSON.ParentUpgradeNames.Count > 0)
                {
                    List<Upgrade> parentUpgrades = new List<Upgrade>();
                    foreach (string parentName in upgradeJSON.ParentUpgradeNames)
                    {
                        if (createdUpgrades.TryGetValue(parentName, out Upgrade parentUpgrade))
                        {
                            parentUpgrades.Add(parentUpgrade);
                        }
                        else
                        {
                            Debug.LogWarning($"Parent upgrade '{parentName}' not found for upgrade '{upgradeJSON.Name}'");
                        }
                    }

                    if (parentUpgrades.Count > 0)
                    {
                        Upgrade currentUpgrade = createdUpgrades[upgradeJSON.Name];
                        var parentUpgradesField = typeof(Upgrade).GetField("ParentUpgrades");
                        parentUpgradesField.SetValue(currentUpgrade, parentUpgrades.ToArray());
                        EditorUtility.SetDirty(currentUpgrade);
                    }
                }
            }

            AssetDatabase.SaveAssets();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting Upgrades JSON: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private static void DeleteExistingScriptableObjects(string folderPath, string filter)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"Folder does not exist: {folderPath}");
            return;
        }

        // Find all assets of the specified type in the folder
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // Don't delete .json files
            if (!assetPath.EndsWith(".json"))
            {
                AssetDatabase.DeleteAsset(assetPath);
                Debug.Log($"Deleted old asset: {assetPath}");
            }
        }
    }

    private static void UpdateEventManagerParameter(List<Event> events)
    {
        EventManagerParameter eventManagerParam = AssetDatabase.LoadAssetAtPath<EventManagerParameter>(EVENT_MANAGER_PARAM_PATH);
        
        if (eventManagerParam == null)
        {
            Debug.LogWarning($"EventManagerParameter not found at: {EVENT_MANAGER_PARAM_PATH}");
            return;
        }

        // Update only the AllEvents list, keep other parameters unchanged
        eventManagerParam.AllEvents = events;
        EditorUtility.SetDirty(eventManagerParam);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Updated EventManagerParameter with {events.Count} events");
    }

    private static void UpdatePromptManagerParameter(List<Prompt> prompts)
    {
        PromptManagerParameter promptManagerParam = AssetDatabase.LoadAssetAtPath<PromptManagerParameter>(PROMPT_MANAGER_PARAM_PATH);
        
        if (promptManagerParam == null)
        {
            Debug.LogWarning($"PromptManagerParameter not found at: {PROMPT_MANAGER_PARAM_PATH}");
            return;
        }

        // Update only the AllPrompts list, keep other parameters unchanged
        promptManagerParam.AllPrompts = prompts;
        EditorUtility.SetDirty(promptManagerParam);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Updated PromptManagerParameter with {prompts.Count} prompts");
    }

    /// <summary>
    /// Strips C-style comments from JSON string to allow JSONC format (JSON with Comments).
    /// Supports both single-line (//) and multi-line (/* */) comments.
    /// </summary>
    private static string StripComments(string jsonWithComments)
    {
        var result = new System.Text.StringBuilder(jsonWithComments.Length);
        bool inString = false;
        bool inSingleLineComment = false;
        bool inMultiLineComment = false;
        char prevChar = '\0';

        for (int i = 0; i < jsonWithComments.Length; i++)
        {
            char c = jsonWithComments[i];
            char nextChar = (i + 1 < jsonWithComments.Length) ? jsonWithComments[i + 1] : '\0';

            // Handle string boundaries (ignore comments inside strings)
            if (c == '"' && prevChar != '\\' && !inSingleLineComment && !inMultiLineComment)
            {
                inString = !inString;
                result.Append(c);
                prevChar = c;
                continue;
            }

            // If we're in a string, just append and continue
            if (inString)
            {
                result.Append(c);
                prevChar = c;
                continue;
            }

            // Check for start of single-line comment
            if (!inMultiLineComment && c == '/' && nextChar == '/')
            {
                inSingleLineComment = true;
                i++; // Skip the next '/'
                prevChar = '/';
                continue;
            }

            // Check for end of single-line comment
            if (inSingleLineComment && (c == '\n' || c == '\r'))
            {
                inSingleLineComment = false;
                result.Append(c); // Keep the newline
                prevChar = c;
                continue;
            }

            // Check for start of multi-line comment
            if (!inSingleLineComment && c == '/' && nextChar == '*')
            {
                inMultiLineComment = true;
                i++; // Skip the next '*'
                prevChar = '*';
                continue;
            }

            // Check for end of multi-line comment
            if (inMultiLineComment && c == '*' && nextChar == '/')
            {
                inMultiLineComment = false;
                i++; // Skip the next '/'
                prevChar = '/';
                continue;
            }

            // If we're in a comment, skip the character
            if (inSingleLineComment || inMultiLineComment)
            {
                prevChar = c;
                continue;
            }

            // Normal character, append it
            result.Append(c);
            prevChar = c;
        }

        return result.ToString();
    }

    private static Sprite LoadIconSprite(string iconFileName)
    {
        if (string.IsNullOrEmpty(iconFileName))
        {
            Debug.LogWarning("Icon file name is null or empty");
            return null;
        }

        // Remove file extension if present
        string iconNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(iconFileName);
        
        // Try to load the sprite from the Icons folder
        string iconPath = $"{ICONS_FOLDER}/{iconNameWithoutExtension}.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        
        if (sprite == null)
        {
            Debug.LogWarning($"Icon sprite not found at: {iconPath}");
        }
        
        return sprite;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            // Try as public field
            var publicField = obj.GetType().GetField(propertyName, 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (publicField != null)
            {
                publicField.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"Could not find field or property: {propertyName} on type {obj.GetType().Name}");
            }
        }
    }

    private static void ResolveEventPlannedActions(EventListJSON eventList, Dictionary<string, Event> createdEvents)
    {
        // Load all prompts that might be referenced
        Dictionary<string, Prompt> allPrompts = LoadAllPromptsFromAssets();

        for (int i = 0; i < eventList.Events.Count; i++)
        {
            var eventJSON = eventList.Events[i];
            if (eventJSON.EventData.PlannedAction != null)
            {
                Event currentEvent = createdEvents[eventJSON.Name];
                var eventDataField = typeof(Event).GetField("EventData");
                EventData eventData = (EventData)eventDataField.GetValue(currentEvent);

                PlannedAction plannedAction = new PlannedAction();
                SetPrivateProperty(plannedAction, "TicksDelay", eventJSON.EventData.PlannedAction.TicksDelay);

                // Resolve planned prompt reference
                if (!string.IsNullOrEmpty(eventJSON.EventData.PlannedAction.PlannedPromptName))
                {
                    if (allPrompts.TryGetValue(eventJSON.EventData.PlannedAction.PlannedPromptName, out Prompt plannedPrompt))
                    {
                        SetPrivateProperty(plannedAction, "PlannedPrompt", plannedPrompt);
                    }
                    else
                    {
                        Debug.LogWarning($"Planned prompt '{eventJSON.EventData.PlannedAction.PlannedPromptName}' not found for event '{eventJSON.Name}'");
                    }
                }

                // Resolve planned event reference
                if (!string.IsNullOrEmpty(eventJSON.EventData.PlannedAction.PlannedEventName))
                {
                    if (createdEvents.TryGetValue(eventJSON.EventData.PlannedAction.PlannedEventName, out Event plannedEvent))
                    {
                        SetPrivateProperty(plannedAction, "PlannedEvent", plannedEvent);
                    }
                    else
                    {
                        Debug.LogWarning($"Planned event '{eventJSON.EventData.PlannedAction.PlannedEventName}' not found for event '{eventJSON.Name}'");
                    }
                }

                SetPrivateProperty(eventData, "PlannedAction", plannedAction);
                EditorUtility.SetDirty(currentEvent);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static void ResolvePromptPlannedActions(PromptListJSON promptList, Dictionary<string, Prompt> createdPrompts)
    {
        // Load all events that might be referenced
        Dictionary<string, Event> allEvents = LoadAllEventsFromAssets();

        for (int i = 0; i < promptList.Prompts.Count; i++)
        {
            var promptJSON = promptList.Prompts[i];
            Prompt currentPrompt = createdPrompts[promptJSON.Name];
            var promptDataField = typeof(Prompt).GetField("PromptData");
            PromptData promptData = (PromptData)promptDataField.GetValue(currentPrompt);
            bool isDirty = false;

            // Resolve PlannedAction for the prompt itself
            if (promptJSON.PromptData.PlannedAction != null)
            {
                PlannedAction plannedAction = new PlannedAction();
                SetPrivateProperty(plannedAction, "TicksDelay", promptJSON.PromptData.PlannedAction.TicksDelay);

                // Resolve planned prompt reference
                if (!string.IsNullOrEmpty(promptJSON.PromptData.PlannedAction.PlannedPromptName))
                {
                    if (createdPrompts.TryGetValue(promptJSON.PromptData.PlannedAction.PlannedPromptName, out Prompt plannedPrompt))
                    {
                        SetPrivateProperty(plannedAction, "PlannedPrompt", plannedPrompt);
                    }
                    else
                    {
                        Debug.LogWarning($"Planned prompt '{promptJSON.PromptData.PlannedAction.PlannedPromptName}' not found for prompt '{promptJSON.Name}'");
                    }
                }

                // Resolve planned event reference
                if (!string.IsNullOrEmpty(promptJSON.PromptData.PlannedAction.PlannedEventName))
                {
                    if (allEvents.TryGetValue(promptJSON.PromptData.PlannedAction.PlannedEventName, out Event plannedEvent))
                    {
                        SetPrivateProperty(plannedAction, "PlannedEvent", plannedEvent);
                    }
                    else
                    {
                        Debug.LogWarning($"Planned event '{promptJSON.PromptData.PlannedAction.PlannedEventName}' not found for prompt '{promptJSON.Name}'");
                    }
                }

                SetPrivateProperty(promptData, "PlannedAction", plannedAction);
                isDirty = true;
            }

            // Resolve PlannedActions for choices
            if (promptJSON.PromptData.Choices != null && promptData.Choices != null)
            {
                for (int j = 0; j < promptJSON.PromptData.Choices.Count && j < promptData.Choices.Length; j++)
                {
                    var choiceJSON = promptJSON.PromptData.Choices[j];
                    if (choiceJSON.PlannedAction != null)
                    {
                        PlannedAction choicePlannedAction = new PlannedAction();
                        SetPrivateProperty(choicePlannedAction, "TicksDelay", choiceJSON.PlannedAction.TicksDelay);

                        // Resolve planned prompt reference for choice
                        if (!string.IsNullOrEmpty(choiceJSON.PlannedAction.PlannedPromptName))
                        {
                            if (createdPrompts.TryGetValue(choiceJSON.PlannedAction.PlannedPromptName, out Prompt plannedPrompt))
                            {
                                SetPrivateProperty(choicePlannedAction, "PlannedPrompt", plannedPrompt);
                            }
                            else
                            {
                                Debug.LogWarning($"Planned prompt '{choiceJSON.PlannedAction.PlannedPromptName}' not found for choice {j} in prompt '{promptJSON.Name}'");
                            }
                        }

                        // Resolve planned event reference for choice
                        if (!string.IsNullOrEmpty(choiceJSON.PlannedAction.PlannedEventName))
                        {
                            if (allEvents.TryGetValue(choiceJSON.PlannedAction.PlannedEventName, out Event plannedEvent))
                            {
                                SetPrivateProperty(choicePlannedAction, "PlannedEvent", plannedEvent);
                            }
                            else
                            {
                                Debug.LogWarning($"Planned event '{choiceJSON.PlannedAction.PlannedEventName}' not found for choice {j} in prompt '{promptJSON.Name}'");
                            }
                        }

                        promptData.Choices[j].PlannedAction = choicePlannedAction;
                        isDirty = true;
                    }
                }
            }

            if (isDirty)
            {
                EditorUtility.SetDirty(currentPrompt);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static Dictionary<string, Prompt> LoadAllPromptsFromAssets()
    {
        Dictionary<string, Prompt> prompts = new Dictionary<string, Prompt>();
        string[] guids = AssetDatabase.FindAssets("t:Prompt", new[] { PROMPTS_FOLDER });
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Prompt prompt = AssetDatabase.LoadAssetAtPath<Prompt>(assetPath);
            if (prompt != null)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                prompts[name] = prompt;
            }
        }
        
        return prompts;
    }

    private static Dictionary<string, Event> LoadAllEventsFromAssets()
    {
        Dictionary<string, Event> events = new Dictionary<string, Event>();
        string[] guids = AssetDatabase.FindAssets("t:Event", new[] { EVENTS_FOLDER });
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Event evt = AssetDatabase.LoadAssetAtPath<Event>(assetPath);
            if (evt != null)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                events[name] = evt;
            }
        }
        
        return events;
    }
}
#endif

