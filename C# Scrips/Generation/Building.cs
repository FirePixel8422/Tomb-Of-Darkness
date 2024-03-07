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
            foreach (int b in buildingsCreated)
            {
                if (b != 1)
                {
                    return false;
                }
            }
            return true;
        }
    }


    public void SetupBuilding()
    {
        connectedBuildings = new List<Building>(maxConnections);
        buildingsCreated = new List<int>(maxConnections);
        paths = new List<List<Node>>(maxConnections);

        DungeonGrid.Instance.NodeFromWorldPoint(transform.position).walkable = false;
    }


    public void GetPath()
    {
        if (connectedBuildings.Count == 0 && Full == false)
        {
            CreateConnections();
        }


        for (int i = 0; i < connectedBuildings.Count; i++)
        {
            if (buildingsCreated[i] == 1)
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
            if (Vector3.Distance(closestEntrance.position, transform.position) < maxDistToBuilding
                && (Mathf.Abs(_buildings[i].gridPos.y) - Mathf.Abs(gridPos.y)) < maxLayerDiff
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
