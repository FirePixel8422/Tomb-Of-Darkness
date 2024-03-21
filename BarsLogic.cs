using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsLogic : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider coll;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rb;

    public Transform camTransform;

    public Vector3 camPos;
    public Quaternion camRot;

    public float camRotSmoothSpeed;
    public float camMoveSmoothSpeed;

    public float gateCloseDelay;
    public float camResetDelay;
    public float finalResetDelay;



    private void Start()
    {
        coll = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        camPos = camTransform.position;
        camRot = camTransform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LetBarsFall());
            coll.enabled = false;
        }
    }

    private IEnumerator LetBarsFall()
    {
        PlayerDeathManager.Instance.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);
        rb.useGravity = true;
        anim.SetBool("Open", false);

        yield return new WaitForSeconds(camResetDelay);
        PlayerDeathManager.Instance.ResetCamToPlayerView(camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(finalResetDelay);
        ThirdPersonCamera.Instance.ChangeCamFollowTransform(ThirdPersonCamera.Instance.camRotPointX);

    }
}
