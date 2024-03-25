using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsLogic : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    public Transform camTransform;
    public Vector3 gateWorldPos;

    public Vector3 camPos;
    public Quaternion camRot;

    public float gateOpenSpeed;
    public bool gateUsed;

    public float camRotSmoothSpeed;
    public float camMoveSmoothSpeed;

    public float gateCloseDelay;
    public float camResetDelay;
    public float finalResetDelay;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        camPos = camTransform.position;
        camRot = camTransform.rotation;

        gateWorldPos = transform.localPosition;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(OpenGate());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && gateUsed == false)
        {
            StartCoroutine(LetBarsFall());
            gateUsed = true;
        }
    }

    private IEnumerator LetBarsFall()
    {
        PlayerDeathManager.Instance.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);
        rb.useGravity = true;

        yield return new WaitForSeconds(camResetDelay);
        PlayerDeathManager.Instance.ResetCamToPlayerView(camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(finalResetDelay);
        ThirdPersonCamera.Instance.ChangeCamFollowTransform(ThirdPersonCamera.Instance.camRotPointX);
        PlayerController.Instance.canMove = true;

    }
    private IEnumerator OpenGate()
    {
        foreach(BoxCollider coll in GetComponentsInChildren<BoxCollider>())
        {
            Destroy(coll);
        }
        rb.useGravity = true;
        rb.isKinematic = true;

        while (transform.localPosition != gateWorldPos)
        {
            transform.localPosition = VectorLogic.InstantMoveTowards(transform.localPosition, gateWorldPos, gateOpenSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
