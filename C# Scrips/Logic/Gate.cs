using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    private PlayerCutsceneManager manager;

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
        manager = PlayerCutsceneManager.Instance;

        camPos = camTransform.position;
        camRot = camTransform.rotation;

        gateWorldPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && gateUsed == false)
        {
            StartCoroutine(Closegate());
            gateUsed = true;
        }
    }

    private IEnumerator Closegate()
    {
        manager.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);
        rb.useGravity = true;

        yield return new WaitForSeconds(camResetDelay);

        manager.OnCutsceneFinished.AddListener(ResetPlayerCamAndFinishCutscene);
        manager.ResetCamToPlayerView(camMoveSmoothSpeed, camRotSmoothSpeed);

    }
    public IEnumerator OpenGate()
    {
        rb.useGravity = false;
        foreach (BoxCollider coll in GetComponentsInChildren<BoxCollider>())
        {
            Destroy(coll);
        }

        manager.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);

        while (transform.localPosition != gateWorldPos)
        {
            transform.localPosition = VectorLogic.InstantMoveTowards(transform.localPosition, gateWorldPos, gateOpenSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);
        manager.OnCutsceneFinished.AddListener(ResetPlayerCamAndFinishCutscene);
        manager.ResetCamToPlayerView(camMoveSmoothSpeed, camRotSmoothSpeed);
    }

    public void ResetPlayerCamAndFinishCutscene()
    {
        ThirdPersonCamera.Instance.ChangeCamFollowTransform(ThirdPersonCamera.Instance.camRotPointX);
        ThirdPersonCamera.Instance.ChangeCamUpdateMode(true);

        PlayerController.Instance.canMove = true;
        manager.OnCutsceneFinished.RemoveAllListeners();
    }
}
