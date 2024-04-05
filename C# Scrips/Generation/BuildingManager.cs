using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public List<Building> buildings;

    public int maxAttempts;
    public int maxReconfigureAttempts;
    public int cReconfigureAttempts;
    public int cBuildingsConfigDone;
    public int cAttempts;

    
    private void Start()
    {
        buildings = FindObjectsOfType<Building>(true).ToList();
        foreach (Building building in buildings)
        {
            building.gameObject.SetActive(true);
        }
        StartCoroutine(StartGeneration());
    }

    private IEnumerator StartGeneration()
    {
        for (int currentAttempt = 0; currentAttempt < maxAttempts; currentAttempt++)
        {

            yield return new WaitForSeconds(0.1f);
            PathFinding.Instance.ResetGenerationSystem();

            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].SetupBuilding();
            }
            

            SortBuildings(out buildings);

            cBuildingsConfigDone = 0;
            cReconfigureAttempts = 0;
            for (int tries = 0; tries < maxReconfigureAttempts; tries++)
            {
                cReconfigureAttempts = tries + 1;
                cBuildingsConfigDone = 0;
                for (int i = 0; i < buildings.Count; i++)
                {
                    if (buildings[i].DoneBuilding == false)
                    {
                        buildings[i].GetPath();
                    }
                    else
                    {
                        cBuildingsConfigDone += 1;
                    }
                }

                if (cBuildingsConfigDone == buildings.Count)
                {
                    cAttempts = currentAttempt + 1;
                    print(buildings.Count + " buildings checked and generated succesfully, took " + cAttempts + " attempts and took " + PathFinding.Instance.totalMsLoadTime + "ms");
                    DungeonGrid.Instance.SpawnCubes();
                    foreach (Building building in buildings)
                    {
                        building.gameObject.SetActive(false);
                    }
                    yield break;
                }
                yield return null;
            }



            int succes = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].buildingsCreated.Contains(1))
                {
                    succes += 1;
                }
            }
            if (succes == buildings.Count)
            {
                cAttempts = currentAttempt + 1;
                print(buildings.Count + " buildings checked and generated succesfully, took " + cAttempts + " attempts and took " + PathFinding.Instance.totalMsLoadTime + "ms");
                DungeonGrid.Instance.SpawnCubes();
                foreach (Building building in buildings)
                {
                    building.gameObject.SetActive(false);
                }
                yield break;
            }

            cAttempts = currentAttempt + 1;
        }



        print(buildings.Count + " buildings checked for connection, failed, Cycle done...");
        DungeonGrid.Instance.SpawnCubes();
        foreach (Building building in buildings)
        {
            building.gameObject.SetActive(false);
        }
    }


    public void SortBuildings(out List<Building> buildings)
    {
        List<Building> sortedList = new List<Building>();
        buildings = this.buildings.ToList();
        
        while (buildings.Count != 0)
        {
            int highestPriority = -10;
            int index = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].priority > highestPriority)
                {
                    highestPriority = buildings[i].gridPos.y;
                    index = i;
                }
            }
            sortedList.Add(buildings[index]);
            buildings.RemoveAt(index);
        }

        buildings = sortedList;
    }
}
