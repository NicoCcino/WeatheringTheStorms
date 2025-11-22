using UnityEngine;

public class GridDebugDisplay : MonoBehaviour
{
    public GameObject WorldMapPlane;
    public Vector3 bottomLeft;
    public int gridWidth;
    public int gridHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float cellSize = 1f; // taille d'une cellule


    void OnDrawGizmos()
    {
        DisplayCoordinates();
    }

    void DisplayCoordinates()
    {
        SpriteRenderer sprite = WorldMapPlane.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            // Récupère les bounds dans le monde
            Bounds bounds = sprite.bounds;

            // Coin inférieur gauche
            bottomLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);

            Debug.Log("Coin inférieur gauche : " + bottomLeft);
        }


        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calcul de la position dans le monde
                Vector3 worldPos = new Vector3(bottomLeft.x + x * cellSize, bottomLeft.y + y * cellSize, 0);

                // Dessiner un petit cube pour visualiser la cellule
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(worldPos + new Vector3(cellSize, cellSize, 0) * 0.5f, new Vector3(cellSize, cellSize, 0));

                // Afficher les coordonnées dans la scène
#if UNITY_EDITOR
                UnityEditor.Handles.Label(worldPos + new Vector3(0, 0.1f, 0), $"({x},{y})");
#endif
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
