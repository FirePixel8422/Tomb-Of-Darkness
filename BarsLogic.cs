using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsLogic : MonoBehaviour
{
    public BoxCollider coll;
    public Animator anim;

    public Transform camTransform;

    public Vector3 localCamPos;
    public Vector3 localRot;

    private void Start()
    {
        localCamPos = camTransform.localPosition;
        localRot = camTransform.localEulerAngles;
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LetBarsFall();
            coll.enabled = false;
        }
    }

    public void LetBarsFall()
    {
        PlayerController.Instance.canMove = false;
        ThirdPersonCamera.Instance.ChangeCamFollowTransform(camTransform);
        anim.SetTrigger("Fall");
    }
}
