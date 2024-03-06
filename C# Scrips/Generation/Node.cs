using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class Node : IHeapItems<Node>
{
    public int3 gridPos;
    public Vector3 worldPos;

    public GameObject tile;

    public bool walkable;

    public int movementPenalty;

    public int3 parentIndex;
    public int partOfStair;

    public List<int3> stairDirList;
    public int3 stairDir;

    public bool isStair;
    public bool isOpen;



    public int gCost;
    public int hCost;
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public void ResetNode()
    {
        walkable = true;
        partOfStair = 0;
        isStair = false;
        isOpen = false;
        isStair = false;
        stairDirList = new List<int3>(4);
        stairDir = 0;
    }


    private int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }
    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}