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

    public float totalMsLoadTime;


    [HideInInspector]
    public DungeonGrid grid;

    public List<Node> path;

    public float targetMoveDistanceForPathUpdate = 1.5f;
    public int ignoredBaseUpdateRange = 25;
    public int rangeForFasterPathUpdateSpeed = 30;



    private void Start()
    {
        grid = DungeonGrid.Instance;
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
    public void FindPath(Vector3 startPos, Vector3 targetPos, out int buildingResult)
    {
        buildingResult = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);


        Heap<Node> openNodes = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();


        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();

            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                buildingResult = 1;
                RetracePath(startNode, targetNode);
                sw.Stop();
                print("Path Found in " + sw.ElapsedMilliseconds + " ms");
                totalMsLoadTime += sw.ElapsedMilliseconds;
                return;
            }

            int stairDirection = 0;
            if (currentNode.isStair == false)
            {
                stairDirection = grid.GetNodeFromGridPos(currentNode.parentIndex).gridPos.y - currentNode.gridPos.y == 1 ? 1 : -1;
            }

            foreach (Node neigbour in grid.GetNeigbours(currentNode, stairDirection))
            {
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                int3 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(int3.zero, currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist + (neigbour.isStair && !neigbour.isOpen ? -10 : 0);
                if (neigbour.isOpen)
                {
                    newMovementCostToNeigbour = -50;
                }

                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(int3.zero, neigbour.gridPos, targetNode.gridPos);

                    neigbour.parentIndex = currentNodeGridPos;

                    if (neigbour.gridPos.y - currentNodeGridPos.y != 0)
                    {
                        neigbour.partOfStair = 1;

                        int3 dir = INT3.Difference(neigbour.gridPos, currentNode.gridPos);
                        neigbour.stairDir = new int3(dir.x, dir.y, dir.z);
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
        }
        print("Path Failed");
    }

    private int GetDistance(int3 mod, int3 gridPosA, int3 gridPosB)
    {
        int distX = Mathf.Abs(gridPosA.x - gridPosB.x);
        int distY = Mathf.Abs(gridPosA.y - gridPosB.y);
        int distZ = Mathf.Abs(gridPosA.z - gridPosB.z);

        return distX * UnityEngine.Random.Range(9, 12)
            + distY * 30
            + distZ * UnityEngine.Random.Range(9, 12);
    }



    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            if (currentNode.partOfStair == 2)
            {
                currentNode.walkable = false;

                int3 clampedStairDir = INT3.Clamp(currentNode.stairDir, -1, 1);
                int3[] disableNodeDirections = new int3[]
                {
                    new int3(clampedStairDir.x, 0, clampedStairDir.z),
                    new int3(clampedStairDir.x * 2, 0, clampedStairDir.z * 2),
                    new int3(clampedStairDir.x, clampedStairDir.y, clampedStairDir.z),
                    new int3(clampedStairDir.x * 2, clampedStairDir.y, clampedStairDir.z),
                };
                for (int i = 0; i < disableNodeDirections.Length; i++)
                {
                    if (currentNode == null)
                    {
                        print("currentnode is null");
                    }
                    print(disableNodeDirections[i] + currentNode.gridPos);
                    grid.GetNodeFromGridPos(disableNodeDirections[i] + currentNode.gridPos).walkable = false;
                }
            }
            currentNode.isOpen = true;

            path.Add(currentNode);
            currentNode = grid.grid[currentNode.parentIndex.y][currentNode.parentIndex.x, currentNode.parentIndex.z];
        }
        path.Add(startNode);
        path.Reverse();

        grid.path.Add(path);
    }
}
