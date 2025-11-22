using UnityEngine;

public class DisplayOnGrid : MonoBehaviour
{

    public Grid grid;
    public GameObject testPrefab;
    public Vector2Int testCoordinates2D;

    public void DisplayObjectOnGrid(GameObject worldPrefab, Vector2Int cellPos)
    {
        Vector3Int cellPos3D = new Vector3Int(cellPos.x, cellPos.y, 0);
        Vector3 worldPos = grid.CellToWorld(cellPos3D);
        Quaternion rot = Quaternion.identity;
        GameObject go = SimplePool.Spawn(worldPrefab, worldPos, rot);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (testPrefab == null) return;
        if (testCoordinates2D == null) return;
        DisplayObjectOnGrid(testPrefab, testCoordinates2D);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
