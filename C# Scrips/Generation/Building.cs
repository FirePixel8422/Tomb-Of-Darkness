using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Building : MonoBehaviour
{
    public List<Transform> entrances;
    public Building[] connectedBuildings;

    public int3 gridPos;

    public int minConnections, maxConnections;
    public float maxDistToBuilding;
    public int maxLayerDiff;

    public bool buildingsCreated;


    private void Start()
    {
        PathFinding.Instance.OnPathFailed.AddListener(() => buildingsCreated = false);
        PathFinding.Instance.OnPathFound.AddListener(() => buildingsCreated = true);
    }

    public void GetPath()
    {
        List<Building> _buildings = FindObjectsOfType<Building>().ToList();
        _buildings.Remove(this);

        List<Building> buildings = new List<Building>();

        for (int i = 0; i < _buildings.Count; i++)
        {
            Transform closestEntrance = _buildings[i].GetClosestEntrance(transform);
            if (Vector3.Distance(closestEntrance.position, transform.position) < maxDistToBuilding
                && Mathf.Abs(_buildings[i].gridPos.y) - Mathf.Abs(gridPos.y) < maxLayerDiff
                && _buildings[i].connectedBuildings.Contains(this) == false)
            {
                buildings.Add(_buildings[i]);
            }
        }

        int connections = UnityEngine.Random.Range(minConnections, maxConnections + 1);
        int amount = Mathf.Min(connections, buildings.Count);

        List<int> numberPot = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            numberPot.Add(i);
        }

        connectedBuildings = new Building[amount];
        for (int i = 0; i < amount; i++)
        {
            int r = UnityEngine.Random.Range(0, numberPot.Count);
            connectedBuildings[i] = buildings[numberPot[r]];
            numberPot.RemoveAt(r);
        }
        for (int i = 0; i < connectedBuildings.Length; i++)
        {
            Transform closestEntranceThisBuilding = GetClosestEntrance(connectedBuildings[i].transform);

            PathFinding.Instance.FindPath(closestEntranceThisBuilding.position, connectedBuildings[i].GetClosestEntrance(closestEntranceThisBuilding).position);
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
