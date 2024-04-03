using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    private PlayerCutsceneManager manager;

    public Gate[] additionalgates;

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
        MusicManager.Instance.ChangeMusicTrack(0, 0.325f);

        manager.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);

        rb.useGravity = true;
        foreach (Gate gate in additionalgates)
        {
            gate.rb.useGravity = true;
            gate.gateUsed = true;
        }

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

        foreach (Gate gate in additionalgates)
        {
            rb.useGravity = false;
            foreach (BoxCollider coll in gate.GetComponentsInChildren<BoxCollider>())
            {
                Destroy(coll);
            }
        }

        manager.PlayCutscene(camPos, camRot, camMoveSmoothSpeed, camRotSmoothSpeed);

        yield return new WaitForSeconds(gateCloseDelay);

        int finishCounter = 0;
        while (finishCounter != 1 + additionalgates.Length)
        {
            finishCounter = 0;

            transform.localPosition = VectorLogic.InstantMoveTowards(transform.localPosition, gateWorldPos, gateOpenSpeed * Time.deltaTime);
            if (transform.localPosition == gateWorldPos)
            {
                finishCounter += 1;
            }

            foreach (Gate gate in additionalgates)
            {
                gate.transform.position = VectorLogic.InstantMoveTowards(gate.transform.position, gate.gateWorldPos, gate.gateOpenSpeed * Time.deltaTime);
                if(gate.transform.localPosition == gate.gateWorldPos)
                {
                    finishCounter += 1;
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);
        manager.OnCutsceneFinished.AddListener(ResetPlayerCamAndFinishCutscene);
        manager.ResetCamToPlayerView(camMoveSmoothSpeed, camRotSmoothSpeed);
    }

    public void ResetPlayerCamAndFinishCutscene()
    {
        rb.isKinematic = true;
        foreach (Gate gate in additionalgates)
        {
            gate.rb.isKinematic = true;
        }
        ThirdPersonCamera.Instance.ChangeCamFollowTransform(ThirdPersonCamera.Instance.camRotPointX);
        ThirdPersonCamera.Instance.ChangeCamUpdateMode(true);

        PlayerController.Instance.canMove = true;
        manager.OnCutsceneFinished.RemoveAllListeners();
    }
}
