
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    // Distance from the center of a hex to one of its corners.
    public float tileSize = 1f;

    public GameObject tilePrefab;

    private Tile[,] grid;

    // Six axial coordinate directions.
    private static readonly Vector2Int[] directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1)
    };

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float worldX = tileSize * 1.5f * x;

                float worldZ = tileSize * Mathf.Sqrt(3f) * (y + (x % 2) * 0.5f);

                Vector3 position = transform.position + new Vector3(worldX, 0f, worldZ);

                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                Tile tile = tileObject.GetComponent<Tile>();

                tile.Initialize(x, y);

                grid[x, y] = tile;
            }
        }

        MissionObjectiveManager objectiveManager = FindAnyObjectByType<MissionObjectiveManager>();

        if (objectiveManager != null)
        {
            objectiveManager.InitializeObjective(this); 
        }
           
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }

        return grid[x, y];
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        Vector2Int[] directions;

        if (tile.x % 2 == 0)
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1)
            };
        }
        else
        {
            directions = new Vector2Int[]
            {
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1)
            };
        }

        foreach (Vector2Int direction in directions)
        {
            int newX = tile.x + direction.x;
            int newY = tile.y + direction.y;

            Tile neighbor = GetTile(newX, newY);

            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    // Calculates distance between two hexagonal tiles.
    public int GetHexDistance(Tile a, Tile b)
    {
        int ax = a.x;
        int az = a.y - (a.x - (a.x & 1)) / 2;

        int bx = b.x;
        int bz = b.y - (b.x - (b.x & 1)) / 2;

        int dx = ax - bx;
        int dz = az - bz;
        int dy = -dx - dz;

        return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
    }
}
