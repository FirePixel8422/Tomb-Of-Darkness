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


    public void OnTriggerEnter(Collider other)
    {
        ThirdPersonCamera.Instance.ChangeCamUpdateMode(false);
        PlayerDeathManager.Instance.KillBySpikes(transform.position);

        trapPlaneTriggerd = true;
        wallColliders.SetActive(true);
    }

    public IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(1);
        wallColliders.SetActive(false);
        trapPlaneTriggerd = false;
    }

    private void FixedUpdate()
    {
        if (trapPlaneTriggerd == true)
        {
            trapPlane.isKinematic = false;
            trapPlane.velocity = new Vector3(0, -fallSpeed, 0);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            print("death");
        }
    }
}
