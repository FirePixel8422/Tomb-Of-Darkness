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
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, out int buildingResult)
    {
        buildingResult = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);


        Heap<Node> openNodes = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                for (int z = 0; z < grid.gridSizeZ; z++)
                {
                    Node node = grid.GetNodeFromGridPos(new int3(x, y, z));
                    node.gCost = 0;
                    node.hCost = 0;
                }
            }
        }


        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();

            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {                
                sw.Stop();
                //print("Path Found in " + sw.ElapsedMilliseconds + " ms");
                totalMsLoadTime += sw.ElapsedMilliseconds;

                buildingResult = 1;
                return RetracePath(startNode, targetNode);
            }

            int stairDirection = 0;
            if (currentNode.isStair == false)
            {
                //stairDirection = grid.GetNodeFromGridPos(currentNode.parentIndex).gridPos.y - targetNode.gridPos.y;
                stairDirection = Mathf.Clamp(targetNode.gridPos.y - currentNode.gridPos.y, -1, 1);
            }

            foreach (Node neigbour in grid.GetNeigbours(currentNode, stairDirection))
            {
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                if (neigbour.gridPos.y - currentNode.gridPos.y != 0)
                {
                    int3 clampedStairDir = -INT3.Clamp(INT3.Difference(neigbour.gridPos, currentNode.gridPos), -1, 1);
                    int3[] disableNodeDirections = new int3[]
                    {
                        new int3(clampedStairDir.x, 0, clampedStairDir.z),
                        new int3(clampedStairDir.x * 2, 0, clampedStairDir.z * 2),
                        new int3(clampedStairDir.x, clampedStairDir.y, clampedStairDir.z),
                        new int3(clampedStairDir.x * 2, clampedStairDir.y, clampedStairDir.z * 2),
                    };

                    int amountOfTilesAvailable = 0;
                    for (int i = 0; i < disableNodeDirections.Length; i++)
                    {                        
                        if (grid.IsInsideGrid(disableNodeDirections[i] + neigbour.gridPos) == false)
                        {
                            break;
                        }
                        Node node = grid.GetNodeFromGridPos(disableNodeDirections[i] + neigbour.gridPos);
                        if ((node.walkable == true && node.isOpen == false) || (currentNode.isStair && INT3.Clamp(currentNode.stairDir, -1, 1).Equals(clampedStairDir)))
                        {
                            amountOfTilesAvailable += 1;
                        }
                    }
                    if (amountOfTilesAvailable < 4)
                    {
                        continue;
                    }
                }

                int3 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(int3.zero, currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist + (neigbour.isOpen ? -10 : 0) + (neigbour.isStair ? +20 : 0);


                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(int3.zero, neigbour.gridPos, targetNode.gridPos) + (neigbour.isOpen ? -10 : 0) + (neigbour.isStair ? +20 : 0);

                    neigbour.parentIndex = currentNodeGridPos;

                    if (neigbour.gridPos.y - currentNodeGridPos.y != 0)
                    {
                        neigbour.partOfStair = 1;

                        int3 dir = INT3.Difference(neigbour.gridPos, currentNode.gridPos);
                        neigbour.stairDir = new int3(dir.x, dir.y, dir.z);
                    }
                    if (currentNode.partOfStair == 1)
                    {
                        grid.GetNodeFromGridPos(currentNode.parentIndex).partOfStair = 2;
                    }


                    if (!openNodes.Contains(neigbour))
                    {
                        openNodes.Add(neigbour);
                    }
                }
            }
        }
        //print("Path Failed");
        totalMsLoadTime += sw.ElapsedMilliseconds;
        return null;
    }

    private int GetDistance(int3 mod, int3 gridPosA, int3 gridPosB)
    {
        int distX = Mathf.Abs(gridPosA.x - gridPosB.x);
        int distY = Mathf.Abs(gridPosA.y - gridPosB.y);
        int distZ = Mathf.Abs(gridPosA.z - gridPosB.z);

        return distX * 10 + UnityEngine.Random.Range(0, 2)
            + distY * 20
            + distZ * 10 + UnityEngine.Random.Range(-1, 1);
    }



    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            if (currentNode.partOfStair == 1)
            {
                currentNode.stairDirList.Add(currentNode.stairDir);

                int3 clampedStairDir = -INT3.Clamp(currentNode.stairDir, -1, 1);
                int3[] disableNodeDirections = new int3[]
                {
                    new int3(clampedStairDir.x, 0, clampedStairDir.z),
                    new int3(clampedStairDir.x * 2, 0, clampedStairDir.z * 2),
                    new int3(clampedStairDir.x, clampedStairDir.y, clampedStairDir.z),
                    new int3(clampedStairDir.x * 2, clampedStairDir.y, clampedStairDir.z * 2),
                };
                for (int i = 0; i < disableNodeDirections.Length; i++)
                {
                    grid.GetNodeFromGridPos(disableNodeDirections[i] + currentNode.gridPos).walkable = false;
                }
            }
            currentNode.isOpen = true;

            path.Add(currentNode);
            currentNode = grid.grid[currentNode.parentIndex.y][currentNode.parentIndex.x, currentNode.parentIndex.z];
        }
        path.Add(startNode);
        startNode.isOpen = true;

        path.Reverse();
        return path;
    }
}
