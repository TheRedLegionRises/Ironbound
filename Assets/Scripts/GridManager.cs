using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    public GameObject tilePrefab;

    private Tile[,] grid;

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
                Vector3 position = new Vector3(
                    x * tileSize,
                    0,
                    y * tileSize
                );

                GameObject tileObject = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                Tile tile = tileObject.GetComponent<Tile>();

                tile.Initialize(x, y);

                grid[x, y] = tile;
            }
        }

        FindAnyObjectByType<MissionObjectiveManager>().InitializeObjective(this);
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= width ||
            y < 0 || y >= height)
        {
            return null;
        }

        return grid[x, y];
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        // Up, down, left, right
        int[,] directions =
        {
            { 0, 1 },
            { 0, -1 },
            { 1, 0 },
            { -1, 0 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = tile.x + directions[i, 0];
            int newY = tile.y + directions[i, 1];

            Tile neighbor = GetTile(newX, newY);

            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }
}
