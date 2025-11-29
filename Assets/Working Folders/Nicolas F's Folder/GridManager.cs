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
    /// <summary>
    /// Checks if a specific coordinate is within the grid boundaries.
    /// </summary>
    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    /// <summary>
    /// Uses Breadth-First Search (BFS) to spiral outwards from the startPosition 
    /// until it finds an empty spot.
    /// </summary>
    public Vector2Int FindClosestAvailablesPosition(Vector2Int startPosition)
    {
        // 1. Check if the requested position is valid and empty right away
        if (IsInsideGrid(startPosition) && IsCoordinatesAvailables(startPosition))
        {
            return startPosition;
        }

        // 2. Setup BFS structures
        // Queue stores the cells we need to check next
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // HashSet keeps track of cells we've already added to the queue to avoid infinite loops
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        // 3. Define search directions. 
        // This includes Up, Down, Left, Right and Diagonals (8 neighbors).
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0),   // Right
            new Vector2Int(1, 1),   // Top-Right
            new Vector2Int(-1, 1),  // Top-Left
            new Vector2Int(1, -1),  // Bottom-Right
            new Vector2Int(-1, -1)  // Bottom-Left
        };

        // 4. Start the search loop
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // Skip if outside grid or already checked
                if (!IsInsideGrid(neighbor) || visited.Contains(neighbor))
                {
                    continue;
                }

                // Check if this neighbor is free
                if (IsCoordinatesAvailables(neighbor))
                {
                    return neighbor; // Found the closest available spot!
                }

                // If occupied, mark as visited and add to queue to check its neighbors later
                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        // 5. Fallback: If the entire grid is full
        Debug.LogWarning("Grid is completely full! No position found.");
        return startPosition;
    }
}

