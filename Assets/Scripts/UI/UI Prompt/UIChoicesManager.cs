using System.Collections.Generic;
using UnityEngine;

public class UIChoicesManager : MonoBehaviour
{
    public List<UIChoice> SpawnedUIChoices = new List<UIChoice>();
    public GameObject choicePrefab;
    public void SpawnChoice(Choice choice, UIPrompt parentUIPrompt)
    {
        GameObject go = SimplePool.Spawn(choicePrefab);
        go.transform.parent = transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = Vector3.zero;
        UIChoice uiChoice = go.GetComponent<UIChoice>();
        uiChoice.UpdateDisplay(choice, parentUIPrompt);
        SpawnedUIChoices.Add(uiChoice);
    }
    public void SpawnChoices(Choice[] choices, UIPrompt parentUIPrompt)
    {
        foreach (UIChoice uiChoice in SpawnedUIChoices)
        {
            SimplePool.Despawn(uiChoice.gameObject);
        }
        for (int i = 0; i < choices.Length; i++)
        {
            SpawnChoice(choices[i], parentUIPrompt);
        }
    }
}

