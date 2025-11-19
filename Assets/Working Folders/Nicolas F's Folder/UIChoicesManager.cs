using UnityEngine;

public class UIChoicesManager : MonoBehaviour
{

    // Responsabilité: ajouter des choix, les cacher, les mettre à jour (contenu)

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject choicePrefab;
    public void SpawnChoice(Choice choice)
    {
        GameObject go = SimplePool.Spawn(choicePrefab);
        go.transform.parent = transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<UIChoice>().UpdateDisplay(choice);
    }



    void Start()
    {
        Choice sampleChoice = new Choice();
        sampleChoice.Label = "Test";
        SpawnChoice(sampleChoice);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
