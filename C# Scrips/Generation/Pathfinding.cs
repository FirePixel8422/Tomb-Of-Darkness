using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance;
    private void Awake()
    {
        Instance = this;
    }

    public UnityEvent OnPathFound;


    [HideInInspector]
    public DungeonGenerator grid;

    public List<Node> path;

    public float targetMoveDistanceForPathUpdate = 1.5f;
    public int ignoredBaseUpdateRange = 25;
    public int rangeForFasterPathUpdateSpeed = 30;



    private void Start()
    {
        grid = DungeonGenerator.Instance;
        grid.Init();
    }
    public void ResetGenerationSystem()
    {
        grid.path.Clear();
        for (int y = 0; y < grid.gridSizeY; y++)
        {
            for (int x = 0; x < grid.gridSizeX; x++)
            {
                for (int z = 0; z < grid.gridSizeZ; z++)
                {
                    grid.grid[y][x, z].ResetNode();
                }
            }
        }
    }
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);


        Heap<Node> openNodes = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        Node currentNode = new Node();

        int stairIndex = 0;

        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            currentNode = openNodes.RemoveFirst();

            closedNodes.Add(currentNode);

            if (currentNode.isStair)
            {
                closedNodes.Add(grid.GetNodeFromGridPos(INT3.Difference(currentNode.gridPos, INT3.Clamp(currentNode.stairDir, -1, 1))));
            }

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                sw.Stop();
                print("Path Found in " + sw.ElapsedMilliseconds + " ms");
                return;
            }

            int stairDirection = 0;
            if (stairIndex == 0)
            {
                stairDirection = grid.GetNodeFromGridPos(currentNode.parentIndex).gridPos.y - currentNode.gridPos.y == 1 ? 1 : -1;
            }

            foreach (Node neigbour in grid.GetNeigbours(currentNode, stairDirection))
            {
                int3 diffPos = INT3.Difference(currentNode.gridPos, neigbour.gridPos);
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                int3 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(int3.zero, currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist + (neigbour.isStair ? 120 : 0);
                

                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(int3.zero, neigbour.gridPos, targetNode.gridPos);
                    
                    neigbour.parentIndex = currentNodeGridPos;

                    if (neigbour.gridPos.y - currentNodeGridPos.y != 0)
                    {
                        neigbour.partOfStair = 1;

                        int3 dir = INT3.Difference(neigbour.gridPos, currentNode.gridPos);
                        neigbour.stairDir = new int3(dir.x, 0, dir.z);
                        stairIndex = 10;
                    }
                    if (currentNode.partOfStair == 1)
                    {
                        neigbour.partOfStair = 2;
                    }


                    if (!openNodes.Contains(neigbour))
                    {
                        openNodes.Add(neigbour);
                    }
                }
            }
            stairIndex -= 1;
        }
        print("Path Failed, trying to recreate now...");
        RetracePath(startNode, currentNode);
    }
    private void RetracePath(Node startNode, Node endNode)
    {
        OnPathFound.Invoke();

        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            if (currentNode.partOfStair != 0)
            {
                currentNode.walkable = false;
            }
            currentNode = grid.grid[currentNode.parentIndex.y][currentNode.parentIndex.x, currentNode.parentIndex.z];
        }
        path.Reverse();

        grid.path.Add(path);
    }

    private int GetDistance(int3 mod, int3 gridPosA, int3 gridPosB)
    {
        int distX = Mathf.Abs(gridPosA.x - gridPosB.x);
        int distY = Mathf.Abs(gridPosA.y - gridPosB.y);
        int distZ = Mathf.Abs(gridPosA.z - gridPosB.z);

        return distX * distX == distZ ? UnityEngine.Random.Range(8,13) : 10
            + distY * UnityEngine.Random.Range(20, 50)
            + distZ * distZ == distX ? UnityEngine.Random.Range(8, 13) : 10;
    }
}
