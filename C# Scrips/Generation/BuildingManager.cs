using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public Building[] buildings;

    public bool fireNextGeneration;

    private void Start()
    {
        buildings = FindObjectsOfType<Building>();
        PathFinding.Instance.OnPathFound.AddListener(FireNextGenerationCall);
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

        int i = 0;
        while(true)
        {
            buildings[i].GetPath();
            yield return new WaitUntil(() => fireNextGeneration == true);
            fireNextGeneration = false;
            i += 1;
            if(i == buildings.Length)
            {
                yield break;
            }
        }
    }
}
