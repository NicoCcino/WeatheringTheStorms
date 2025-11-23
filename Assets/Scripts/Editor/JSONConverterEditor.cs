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
                
                SetPrivateProperty(promptData, "Coordinates", promptJSON.PromptData.Coordinates?.ToVector2Int() ?? Vector2Int.zero);
                SetPrivateProperty(promptData, "DateCondition", promptJSON.PromptData.DateCondition?.ToDateCondition());
                SetPrivateProperty(promptData, "GaugeCondition", promptJSON.PromptData.GaugeCondition?.ToGaugeCondition());
                
                promptDataField.SetValue(promptAsset, promptData);

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
}
#endif

