using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance;
    private void Awake()
    {
        Instance = this;
    }


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
        ResetGenerationSystem();

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);


        Heap<Node> openNodes = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();
        

        int stairIndex = 0;

        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();

            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                sw.Stop();
                print("Path Found in " + sw.ElapsedMilliseconds + " ms");
                return;
            }


            foreach (Node neigbour in grid.GetNeigbours(currentNode, stairIndex == 0))
            {
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                int3 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(int3.zero, currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist + (neigbour.isStair ? 80 : 0);
                

                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(int3.zero, neigbour.gridPos, targetNode.gridPos);
                    
                    neigbour.parentIndex = currentNodeGridPos;

                    if (neigbour.gridPos.x == currentNodeGridPos.x && neigbour.gridPos.z == currentNodeGridPos.z)
                    {
                        neigbour.partOfStair = 1;

                        int3 dir = INT3.Subtract(neigbour.gridPos, grid.GetNodeFromGridPos(currentNode.parentIndex).gridPos);
                        neigbour.stairDir = new int3(dir.x, 0, dir.z);
                        neigbour.stairLogicTimer = 3;

                        stairIndex = 2;
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

            if (INT3.IsZero(currentNode.stairDir) == false)
            {
                currentNode.stairLogicTimer -= 1;
                if (currentNode.stairLogicTimer == 0)
                {
                    currentNode.stairDir = int3.zero;
                }
            }
            stairIndex -= 1;
        }
    }
    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = grid.grid[currentNode.parentIndex.y][currentNode.parentIndex.x, currentNode.parentIndex.z];
        }
        path.Reverse();

        grid.path = path;
    }

    private int GetDistance(int3 mod, int3 gridPosA, int3 gridPosB)
    {
        int distX = Mathf.Abs(gridPosA.x - gridPosB.x);
        int distY = Mathf.Abs(gridPosA.y - gridPosB.y);
        int distZ = Mathf.Abs(gridPosA.z - gridPosB.z);

        return distX * distX == distZ ? UnityEngine.Random.Range(8,13) : 10
            + distY * UnityEngine.Random.Range(-2, 16)
            + distZ * distZ == distX ? UnityEngine.Random.Range(8, 13) : 10;
    }
}
