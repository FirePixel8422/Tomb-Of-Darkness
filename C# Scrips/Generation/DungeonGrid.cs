using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGrid : MonoBehaviour
{
    public static DungeonGrid Instance;
    private void Awake()
    {
        Instance = this;
    }


    public bool drawGizmos;
    public Color cellColor;

    public List<List<Node>> path = new List<List<Node>>();
    public NodeGizmo[] pathGizmoRules;

    public Vector3Int gridSize;
    public float tileSize;

    public List<Node[,]> grid;
    public int gridSizeX, gridSizeY, gridSizeZ;
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }



    public void Init()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        grid = new List<Node[,]>();

        gridSizeX = Mathf.RoundToInt(gridSize.x / tileSize);
        gridSizeY = Mathf.RoundToInt(gridSize.y / tileSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / tileSize);

        Vector3 worldBottomLeft = Vector3.zero - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2 - Vector3.up * (gridSize.y / 2);

        for (int y = 0; y < gridSizeY; y++)
        {
            grid.Add(new Node[gridSizeX, gridSizeZ]);

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPos = worldBottomLeft
                        + Vector3.right * (x * tileSize + tileSize / 2)
                        + Vector3.forward * (z * tileSize + tileSize / 2)
                        + Vector3.up * (y * tileSize + tileSize / 2);

                    grid[y][x, z] = new Node()
                    {
                        gridPos = new int3(x, y, z),
                        worldPos = worldPos,
                        walkable = true,
                    };
                }
            }
        }
    }

    public List<Node> GetNeigbours(Node node, int getStairDirection)
    {
        List<Node> neigbours = new List<Node>();

        int3 gridPos = node.gridPos;
        List<int3> directions = new List<int3>()
        {
            new int3(1, 0, 0) + gridPos,
            new int3(-1, 0, 0) + gridPos,
            new int3(0, 0, 1) + gridPos,
            new int3(0, 0, -1) + gridPos,
        };


        if (getStairDirection == 1)
        {
            directions.Add(new int3(3, 1, 0) + gridPos);
            directions.Add(new int3(-3, 1, 0) + gridPos);
            directions.Add(new int3(0, 1, 3) + gridPos);
            directions.Add(new int3(0, 1, -3) + gridPos);
        }
        if (getStairDirection == -1)
        {
            directions.Add(new int3(3, -1, 0) + gridPos);
            directions.Add(new int3(-3, -1, 0) + gridPos);
            directions.Add(new int3(0, -1, 3) + gridPos);
            directions.Add(new int3(0, -1, -3) + gridPos);
        }



        foreach (int3 dir in directions)
        {
            if (IsInsideGrid(dir) == false)
            {
                continue;
            }
            Node targetNode = grid[dir.y][dir.x, dir.z];
            if (dir.y != 0)
            {
                targetNode.isStair = true;
            }
            neigbours.Add(targetNode);
        }

        return neigbours;
    }
    public bool IsInsideGrid(int3 gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= gridSizeX || gridPos.z < 0 || gridPos.z >= gridSizeZ || gridPos.y < 0 || gridPos.y >= gridSizeY)
        {
            return false;
        }
        return true;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;
        float percentZ = (worldPosition.z + gridSize.z / 2) / gridSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[y][x, z];
    }

    public Node GetNodeFromGridPos(int3 gridPos)
    {
        if (!IsInsideGrid(gridPos))
        {
            return null;
        }
        return grid[gridPos.y][gridPos.x, gridPos.z];
    }




    public void OnDrawGizmos()
    {
        Gizmos.color = cellColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(gridSize.x, gridSize.y, gridSize.z));

        gridSizeX = Mathf.RoundToInt(gridSize.x / tileSize);
        gridSizeY = Mathf.RoundToInt(gridSize.y / tileSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / tileSize);

        if (drawGizmos && Application.isPlaying)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        Gizmos.DrawCube(grid[y][x, z].worldPos, Vector3.one * tileSize * 0.3f);
                    }
                }
            }
        }
        for (int i = 0; i < path.Count; i++)
        {
            int gizmoIndex = i;
            while (gizmoIndex >= pathGizmoRules.Length)
            {
                gizmoIndex -= pathGizmoRules.Length;
            }

            if (pathGizmoRules[gizmoIndex].visible == false)
            {
                continue;
            }
            for (int i2 = 0; i2 < path[i].Count; i2++)
            {
                Gizmos.color = pathGizmoRules[gizmoIndex].color;
                if (i2 + 1 != path[i].Count)
                {
                    if (path[i][i2 + 1].partOfStair == 1)
                    {
                        Gizmos.color = Color.black;
                    }
                    if (path[i][i2 + 1].partOfStair == 2)
                    {
                        Gizmos.color = Color.gray;
                    }
                }
                Gizmos.DrawCube(path[i][i2].worldPos, Vector3.one * tileSize);
            }
        }
    }


    [System.Serializable]
    public class NodeGizmo 
    { 
        public Color color;
        public bool visible = true;
    }
}