using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for PromptBalancer to provide a better user interface in the Inspector
/// </summary>
[CustomEditor(typeof(PromptBalancer))]
public class PromptBalancerEditor : Editor
{
    private Vector2 scrollPosition = Vector2.zero;

    public override void OnInspectorGUI()
    {
        PromptBalancer balancer = (PromptBalancer)target;

        // Title
        EditorGUILayout.Space(10);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUILayout.LabelField("PROMPT BALANCER", titleStyle);
        EditorGUILayout.Space(10);

        // Configuration Section
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        
        SerializedProperty promptManagerProp = serializedObject.FindProperty("promptManagerParameter");
        EditorGUILayout.PropertyField(promptManagerProp, new GUIContent("Prompt Manager Parameter"));
        
        if (promptManagerProp.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("⚠ Please assign a PromptManagerParameter!", MessageType.Warning);
        }
        
        EditorGUILayout.Space(5);
        
        SerializedProperty promptBalancerParamProp = serializedObject.FindProperty("promptBalancerParameter");
        EditorGUILayout.PropertyField(promptBalancerParamProp, new GUIContent("Prompt Balancer Parameter"));
        
        if (promptBalancerParamProp.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("⚠ Please assign a PromptBalancerParameter!", MessageType.Warning);
        }
        else
        {
            // Display the current target values from the PromptBalancerParameter (read-only)
            PromptBalancerParameter balancerParam = (PromptBalancerParameter)promptBalancerParamProp.objectReferenceValue;
            
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Current Target Values (from PromptBalancerParameter)", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            
            if (balancerParam.gameBalancingParameter != null)
            {
                EditorGUILayout.LabelField("Climate/Societal", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"  Target Mean: {balancerParam.gameBalancingParameter.targetGaugeEsperanceValue}");
                EditorGUILayout.LabelField($"  Target Amplitude: {balancerParam.gameBalancingParameter.targetGaugeMaxAmplitude}");
            }
            else
            {
                EditorGUILayout.HelpBox("GameBalancingParameter not assigned in PromptBalancerParameter", MessageType.Warning);
            }
            
            EditorGUILayout.LabelField("Trust", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"  Target Mean: {balancerParam.targetTrustEsperanceValue}");
            EditorGUILayout.LabelField($"  Target Amplitude: {balancerParam.targetTrustMaxAmplitude}");
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space(10);
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(15);

        // Manual Balance Button
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            fixedHeight = 35
        };
        
        if (GUILayout.Button("▶ BALANCE ALL PROMPTS NOW", buttonStyle))
        {
            if (promptManagerProp.objectReferenceValue == null)
            {
                EditorUtility.DisplayDialog(
                    "Cannot Balance",
                    "Please assign a PromptManagerParameter before running the balancer.",
                    "OK"
                );
            }
            else if (promptBalancerParamProp.objectReferenceValue == null)
            {
                EditorUtility.DisplayDialog(
                    "Cannot Balance",
                    "Please assign a PromptBalancerParameter before running the balancer.",
                    "OK"
                );
            }
            else
            {
                balancer.BalanceAllPrompts();
                Repaint(); // Refresh the Inspector to show the report
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);

        // Statistics (if in play mode or after balancing)
        if (promptManagerProp.objectReferenceValue != null)
        {
            PromptManagerParameter param = (PromptManagerParameter)promptManagerProp.objectReferenceValue;
            if (param.AllPrompts != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Total Prompts: {param.AllPrompts.Count}");
                
                int totalChoices = 0;
                int promptsWithChoices = 0;
                int linkedEventsCount = 0;
                foreach (var prompt in param.AllPrompts)
                {
                    if (prompt != null && prompt.PromptData != null && prompt.PromptData.Choices != null)
                    {
                        totalChoices += prompt.PromptData.Choices.Length;
                        if (prompt.PromptData.Choices.Length > 0)
                            promptsWithChoices++;
                        
                        foreach (var choice in prompt.PromptData.Choices)
                        {
                            if (choice.PlannedAction != null && choice.PlannedAction.PlannedEvent != null)
                                linkedEventsCount++;
                        }
                    }
                }
                
                EditorGUILayout.LabelField($"Total Choices: {totalChoices}");
                EditorGUILayout.LabelField($"Prompts with Choices: {promptsWithChoices}");
                EditorGUILayout.LabelField($"Linked Events: {linkedEventsCount}");
                EditorGUILayout.LabelField($"Average Choices per Prompt: {(promptsWithChoices > 0 ? (float)totalChoices / promptsWithChoices : 0):F1}");
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.Space(10);

        // Report Display
        if (balancer.hasReport)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle reportTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("PROMPT BALANCER - AUTO-SCALING REPORT", reportTitleStyle);
            EditorGUILayout.Space(5);
            
            // Summary
            GUIStyle summaryStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                fontSize = 10
            };
            EditorGUILayout.LabelField(balancer.reportSummary, summaryStyle);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Details:", EditorStyles.boldLabel);
            
            // Scrollable details area
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));
            
            GUIStyle detailStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 9,
                richText = true
            };
            
            foreach (string line in balancer.reportDetails)
            {
                EditorGUILayout.LabelField(line, detailStyle);
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("No report yet. Click the button above to balance prompts.", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }
    }
}
