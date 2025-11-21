using UnityEngine;

public class AddChoice : MonoBehaviour
{

    [SerializeField] private GameObject ChoicePrefab;
    private void SpawnChoice(Choice choice)
    {
        GameObject go = SimplePool.Spawn(ChoicePrefab);
        Transform component = go.GetComponent<Transform>();
        component.DisplayChoice(choice);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
