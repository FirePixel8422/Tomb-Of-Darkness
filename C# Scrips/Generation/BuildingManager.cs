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

    public bool fireNextGeneration;

    
    private void Start()
    {
        buildings = FindObjectsOfType<Building>();
        PathFinding.Instance.OnPathResult.AddListener(FireNextGenerationCall);
        StartCoroutine(StartGeneration());
    }

    public void FireNextGenerationCall()
    {
        fireNextGeneration = true;
    }

    private IEnumerator StartGeneration()
    {
        yield return new WaitForSeconds(0.1f);
        PathFinding.Instance.ResetGenerationSystem();



        while(true)
        {

            int buildingsDone = 0;
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].buildingsCreated == false)
                {
                    buildings[i].GetPath();
                }
                else
                {
                    buildingsDone += 1;
                }
                yield return new WaitUntil(() => fireNextGeneration == true);
                fireNextGeneration = false;
            }

            if(buildingsDone == buildings.Length)
            {
                break;
            }
        }
        
        print(buildings.Length + " building checked for connection");
    }
}
