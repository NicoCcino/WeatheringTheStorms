using UnityEngine;

public class BubblesSpawner : MonoBehaviour
{

    public GameObject bubblePrefab;
    public float spawnProba;
    public GameObject spawnArea;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to the OnTick event from Timeline
        if (Timeline.Instance != null)
        {
            Timeline.Instance.OnTick += OnTimelineTick;
        }
    }

    public void OnTimelineTick(uint currentTick)
    {

        float r = Random.value;

        if (r < spawnProba)
        {
            Vector3 pos = Vector3.zero;
            pos = GetRandomPoint(spawnArea.transform);
            Quaternion rotation = Quaternion.identity;
            GameObject go = SimplePool.Spawn(bubblePrefab, pos, rotation);
            go.transform.parent = transform;
            // Spawn Bubble;
        }
        else
        {
            // Don't do anything
        }
    }

    public Vector3 GetRandomPoint(Transform planeTransform)
    {
        // Récupère les dimensions du plane en tenant compte du scale
        float width = 10f * planeTransform.localScale.x;
        float height = 10f * planeTransform.localScale.z;

        // Position locale aléatoire
        float x = Random.Range(-width / 2f, width / 2f);
        float z = Random.Range(-height / 2f, height / 2f);

        // Convertit en coordonnées mondiales
        return planeTransform.TransformPoint(new Vector3(x, 0f, z));
    }



    // Update is called once per frame
    void Update()
    {

    }
}
