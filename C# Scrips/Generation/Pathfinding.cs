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
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        int3 pos = startNode.gridPos + new int3(0, 1, 0);
        if (grid.IsInsideGrid(pos))
        {
            grid.grid[pos.y][pos.x, pos.z].walkable = false;
        }

        pos -= new int3(0, 2, 0);
        if(grid.IsInsideGrid(pos))
        {
            grid.grid[pos.y][pos.x, pos.z].walkable = false;
        }


        Heap<Node> openNodes = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        int[] movPenalties = new int[3];


        int3 stairDir = int3.zero;
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

            if (stairIndex == 0)
            {
                stairDir = int3.zero;
            }
            else
            {
                stairIndex -= 1;
            }


            foreach (Node neigbour in grid.GetNeigbours(currentNode, currentNode.partOfStair > 0, stairDir))
            {

                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }
                movPenalties[0] = neigbour.movementPenalty;
                movPenalties[1] = currentNode.movementPenalty;
                movPenalties[2] = targetNode.movementPenalty;

                int3 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(movPenalties[1], movPenalties[0], currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist + movPenalties[0] / 10 * neigbourDist;
                

                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(movPenalties[0], movPenalties[2], neigbour.gridPos, targetNode.gridPos);
                    
                    neigbour.parentIndex = currentNodeGridPos;

                    if (neigbour.gridPos.x == currentNodeGridPos.x && neigbour.gridPos.z == currentNodeGridPos.z)
                    {
                        neigbour.partOfStair = 1;

                        int3 dir = new int3(currentNode.gridPos.x - neigbour.gridPos.x, currentNode.gridPos.y - neigbour.gridPos.y, currentNode.gridPos.z - neigbour.gridPos.z);
                        neigbour.stairDir = dir;
                        stairIndex = 2;
                        stairDir = dir;
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

    private int GetDistance(int movPenaltyA, int movPenaltyB, int3 gridPosA, int3 gridPosB)
    {
        int distX = Mathf.Abs(gridPosA.x - gridPosB.x);
        int distY = Mathf.Abs(gridPosA.y - gridPosB.y);
        int distZ = Mathf.Abs(gridPosA.z - gridPosB.z);

        return distX * distX == distZ ? UnityEngine.Random.Range(5,16) : 10
            + distY * UnityEngine.Random.Range(-2, 16)
            + distZ * distZ == distX ? UnityEngine.Random.Range(5, 16) : 10;
    }
}
