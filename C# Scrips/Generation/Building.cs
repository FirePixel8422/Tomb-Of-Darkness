using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Building : MonoBehaviour
{
    public List<Transform> entrances;

    public List<Building> connectedBuildings;
    public List<int> buildingsCreated;

    public List<List<Node>> paths;


    public int3 gridPos;

    public int minCreateConnections, maxCreateConnections;
    public int maxConnections;

    public float maxDistToBuilding;
    public int maxLayerDiff;
    public bool forceStairs;

    public GameObject cube;

    public bool Full
    {
        get
        {
            return maxConnections == connectedBuildings.Count;
        }
    }
    public bool DoneBuilding
    {
        get
        {
            if (buildingsCreated.Count == 0)
            {
                return false;
            }
            int amount = minCreateConnections;
            foreach (int b in buildingsCreated)
            {
                if (b != 0)
                {
                    amount -= 1;
                }
                if (amount <= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool special;


    public void SetupBuilding()
    {
        connectedBuildings = new List<Building>(maxConnections);
        buildingsCreated = new List<int>(maxConnections);
        paths = new List<List<Node>>(maxConnections);

        gridPos = DungeonGrid.Instance.NodeFromWorldPoint(transform.position).gridPos;

        float tileSize = DungeonGrid.Instance.tileSize;

        Vector3 bottomLeft = transform.position
            - Vector3.right * transform.localScale.x / tileSize / 2
            - Vector3.forward * transform.localScale.z / tileSize / 2;

        for (int x = 0; x < transform.localScale.x / tileSize; x++)
        {
            for (int y = 0; y < transform.localScale.y / tileSize; y++)
            {
                for (int z = 0; z < transform.localScale.z / tileSize; z++)
                {
                    Node node = DungeonGrid.Instance.NodeFromWorldPoint(bottomLeft
                        + Vector3.right * (x * tileSize - tileSize / 2) + Vector3.forward * (z * tileSize - tileSize / 2));
                    node.walkable = false;

                    if (special)
                    {
                        Node noder = DungeonGrid.Instance.NodeFromWorldPoint(bottomLeft
                        + new Vector3(x * tileSize - tileSize / 2, 0, z * tileSize - tileSize / 2));
                        print(noder.worldPos);
                        Instantiate(cube, node.worldPos, Quaternion.identity);
                    }
                }
            }
        }
    }


    public void GetPath()
    {
        if ((connectedBuildings.Count == 0 && Full == false) || (buildingsCreated.Contains(0) == false && buildingsCreated.Contains(1) == false))
        {
            CreateConnections();
        }


        for (int i = 0; i < connectedBuildings.Count; i++)
        {
            if (buildingsCreated[i] != 0)
            {
                continue;
            }
            Transform closestEntranceThisBuilding = GetClosestEntrance(connectedBuildings[i].transform);

            paths.Add(PathFinding.Instance.FindPath(closestEntranceThisBuilding.position,
                connectedBuildings[i].GetClosestEntrance(closestEntranceThisBuilding).position,
                out int tempInt));
            buildingsCreated[i] = tempInt;
        }
    }


    public void CreateConnections()
    {
        List<Building> _buildings = FindObjectsOfType<Building>().ToList();
        _buildings.Remove(this);

        List<Building> buildings = new List<Building>();

        for (int i = 0; i < _buildings.Count; i++)
        {
            Transform closestEntrance = _buildings[i].GetClosestEntrance(transform);
            int layerDiff = INT3.Difference(_buildings[i].gridPos, gridPos).y;

            if (forceStairs && layerDiff == 0)
            {
                continue;
            }
            if (Vector3.Distance(closestEntrance.position, transform.position) < maxDistToBuilding
                && layerDiff <= maxLayerDiff
                && _buildings[i].connectedBuildings.Contains(this) == false
                && _buildings[i].Full == false)
            {
                buildings.Add(_buildings[i]);
            }
        }


        int connections = UnityEngine.Random.Range(minCreateConnections, maxCreateConnections + 1);
        int amount = Mathf.Min(new int[] { connections, buildings.Count, maxConnections - connectedBuildings.Count});

        List<int> numberPot = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            numberPot.Add(i);
        }

        
        for (int i = 0; i < amount; i++)
        {
            int r = UnityEngine.Random.Range(0, numberPot.Count);

            connectedBuildings.Add(buildings[numberPot[r]]);
            buildingsCreated.Add(0);

            connectedBuildings[i].connectedBuildings.Add(this);
            connectedBuildings[i].buildingsCreated.Add(2);

            numberPot.RemoveAt(r);
        }
    }


    public Transform GetClosestEntrance(Transform targetBuilding)
    {
        float dist = int.MaxValue;
        int index = 0;
        for (int i = 0; i < entrances.Count; i++)
        {
            float newDist = Vector3.Distance(entrances[i].position, targetBuilding.position);
            if (newDist < dist)
            {
                dist = newDist;
                index = i;
            }
        }

        return entrances[index];
    }
}
