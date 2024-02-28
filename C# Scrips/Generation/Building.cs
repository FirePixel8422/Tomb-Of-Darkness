using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    public List<Transform> entrances;



    public Transform GetClosestEntrance(Transform targetDestination)
    {
        float dist = int.MaxValue;
        int index = 0;
        for (int i = 0; i < entrances.Count; i++)
        {
            float newDist = Vector3.Distance(entrances[i].position, targetDestination.position);
            if (newDist < dist)
            {
                dist = newDist;
                index = i;
            }
        }

        return entrances[index];
    }
}
