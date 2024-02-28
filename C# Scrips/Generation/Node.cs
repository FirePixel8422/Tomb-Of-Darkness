using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class Node : IHeapItems<Node>
{
    public int3 gridPos;
    public Vector3 worldPos;

    public bool walkable;

    public int movementPenalty;

    public int3 parentIndex;
    public int partOfStair;
    public int3 stairDir;


    public int gCost;
    public int hCost;
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
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