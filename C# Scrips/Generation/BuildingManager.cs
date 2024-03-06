using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Building[] buildings;

    public int maxAttempts;
    public int cAttempts;

    
    private void Start()
    {
        buildings = FindObjectsOfType<Building>();
        StartCoroutine(StartGeneration());
    }

    private IEnumerator StartGeneration()
    {
        for (int currentAttempt = 0; currentAttempt < maxAttempts; currentAttempt++)
        {
            cAttempts = currentAttempt;

            yield return new WaitForSeconds(0.1f);
            PathFinding.Instance.ResetGenerationSystem();


            for (int i = 0; i < buildings.Length; i++)
            {
                buildings[i].SetupBuilding();
            }

            for (int tries = 0; tries < 5; tries++)
            {
                int buildingsDone = 0;
                for (int i = 0; i < buildings.Length; i++)
                {
                    if (buildings[i].DoneBuilding == false)
                    {
                        buildings[i].GetPath();
                    }
                    else
                    {
                        buildingsDone += 1;
                    }
                }

                if (buildingsDone == buildings.Length)
                {
                    print("All " + buildings.Length + " checked and generated succesfully");
                    DungeonGrid.Instance.SpawnCubes();
                    yield break;
                }
                yield return new WaitForSeconds((buildings.Length - buildingsDone) / 15);
            }



            int succes = 0;
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].buildingsCreated.Contains(1))
                {
                    succes += 1;
                }
            }
            if(succes == buildings.Length)
            {
                print("All " + buildings.Length + " checked and generated succesfully");
                DungeonGrid.Instance.SpawnCubes();
                yield break;
            }
        }



        print(buildings.Length + " buildings checked for connection, failed, Cycle done...");
        DungeonGrid.Instance.SpawnCubes();
    }
}
