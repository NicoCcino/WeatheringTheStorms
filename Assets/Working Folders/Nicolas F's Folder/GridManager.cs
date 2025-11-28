using UnityEngine;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridManager : Singleton<GridManager>
{
    [Header("Grid settings")]
    public Grid grid;                // Grid natif Unity
    public int gridWidth = 10;       // nombre de cellules en X
    public int gridHeight = 10;      // nombre de cellules en Y
    public SpriteRenderer WorldMapSprite; // La référence de l'image de map posée dans le monde.
    public Vector3 gridOriginBottomLeft;

    public Dictionary<Vector2Int, GameObject> OccupiedCoordinates = new Dictionary<Vector2Int, GameObject>();



    [ContextMenu("Reset Grid Origin From Sprite")]
    public void ResetGridOrigin()
    {
        gridOriginBottomLeft = GetBottomLeftOfSprite(WorldMapSprite);
        Debug.Log("Grid origin reset to: " + gridOriginBottomLeft);
        grid.transform.position = gridOriginBottomLeft;
    }
    private void OnDrawGizmos()
    {
        if (grid == null) return;
        if (WorldMapSprite == null) return;

        if (gridOriginBottomLeft == Vector3.zero)
        {
            ResetGridOrigin();
        }

        DisplayGrid();
    }
    void DisplayGrid()
    {
        float cellSize = grid.cellSize.x; // On part du principe de cellules carrées
        Vector3 origin = grid.transform.position + gridOriginBottomLeft;

        Gizmos.color = Color.yellow;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Convertit la coordonnée de cellule → position dans le monde
                Vector3 worldPos = grid.CellToWorld(new Vector3Int(x, y, 0));

                // Dessiner le contour de la cellule
                Gizmos.DrawWireCube(
                    worldPos + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0),
                    new Vector3(cellSize, cellSize, 0)
                );

#if UNITY_EDITOR
                // Afficher le label (coordonnées)
                Handles.Label(
                    worldPos + new Vector3(0.1f, 0.1f, 0),
                    $"({x},{y})"
                );
#endif
            }
        }
    }
    public Vector3 GetBottomLeftOfSprite(SpriteRenderer sprite)
    {
        // Bounds = boite englobante dans le monde
        Bounds b = sprite.bounds;

        // Le coin inférieur gauche du sprite dans le monde
        Vector3 bottomLeft = new Vector3(b.min.x, b.min.y, sprite.transform.position.z);

        return bottomLeft;
    }
    public void DisplayObjectOnGrid(GameObject gameObject, Vector2Int coordinates)
    {
        if (OccupiedCoordinates.TryGetValue(coordinates, out GameObject occupyingGo))
        {
            Debug.LogWarning($"The coordinate {coordinates} is already occupied by {occupyingGo.name}, cant spawn the grid object {gameObject.name}. Need to have a queue on each coordinates");
            return;
        }
        Vector3Int cellPos3D = new Vector3Int(coordinates.x, coordinates.y, 0);
        Vector3 worldPos = grid.CellToWorld(cellPos3D);
        Quaternion rot = Quaternion.identity;

        gameObject.transform.position = worldPos;
        gameObject.transform.rotation = rot;

        OccupiedCoordinates.Add(coordinates, gameObject);
    }
    public void RemoveObjectAtCoordinates(Vector2Int coordinates)
    {
        if (!OccupiedCoordinates.TryGetValue(coordinates, out GameObject occupyingGo))
        {
            Debug.LogWarning($"You tried to remove a gameobject at coordinates {coordinates} but these coordinates are already empty");
            return;
        }
        OccupiedCoordinates.Remove(coordinates);
    }

    public bool IsCoordinatesAvailables(Vector2Int coordinates)
    {
        return !OccupiedCoordinates.TryGetValue(coordinates, out GameObject occupyingGo);
    }
    public Vector2Int GetAvailableRandomPositionOnGrid()
    {
        Vector2Int coordinates = new Vector2Int(UnityEngine.Random.Range(4, gridWidth - 4), UnityEngine.Random.Range(2, gridHeight - 2));
        while (!IsCoordinatesAvailables(coordinates))
        {
            coordinates = new Vector2Int(UnityEngine.Random.Range(4, gridWidth - 4), UnityEngine.Random.Range(2, gridHeight - 2));
        }
        return coordinates;
    }
}

