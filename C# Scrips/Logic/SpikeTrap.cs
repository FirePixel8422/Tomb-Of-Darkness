using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float fallSpeed;

    public Rigidbody trapPlane;
    public bool trapPlaneTriggerd;

    public GameObject wallColliders;

    public BoxCollider trigger;

    public float camRotSmoothSpeed;
    public float camMoveSmoothSpeed;


    public void OnTriggerEnter(Collider other)
    {
        if (trapPlaneTriggerd)
        {
            return;   
        }
        PlayerCutsceneManager.Instance.KillBySpikes(transform.position, camMoveSmoothSpeed, camRotSmoothSpeed);

        trapPlane.isKinematic = false;
        trapPlane.velocity = new Vector3(0, -fallSpeed, 0);
        trapPlaneTriggerd = true;
        wallColliders.SetActive(true);
        StartCoroutine(ResetTrap());
    }

    public IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(10);
        wallColliders.SetActive(false);
        trapPlaneTriggerd = false;
    }
}
