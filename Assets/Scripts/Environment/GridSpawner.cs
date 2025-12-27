using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int depth = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private GameObject tilePrefab;

    private void Start()
    {
        var grid = CreateGrid(width, depth, cellSize);
        SpawnFromGrid(grid, tilePrefab);
    }

    public Vector3[,] CreateGrid(int width, int depth, float cellSize)
    {
        Vector3[,] grid = new Vector3[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                grid[x, z] = new Vector3(x * cellSize, 0f, z * cellSize);
            }
        }

        return grid;
    }

    public void SpawnFromGrid(Vector3[,] grid, GameObject prefab)
    {
        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Instantiate(prefab, grid[x, z], Quaternion.identity, transform);
            }
        }
    }
}
