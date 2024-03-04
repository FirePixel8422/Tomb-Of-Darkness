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

    
    private void Start()
    {
        buildings = FindObjectsOfType<Building>();
        StartCoroutine(StartGeneration());
    }

    private IEnumerator StartGeneration()
    {
        yield return new WaitForSeconds(0.1f);
        PathFinding.Instance.ResetGenerationSystem();



        for (int tries = 0; tries < maxAttempts; tries++)
        {
            print("(Re)Generation Attempt: " + (tries + 1));

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
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
        
        print(buildings.Length + " buildings checked for connection, failed, Cycle done...");
    }
}
