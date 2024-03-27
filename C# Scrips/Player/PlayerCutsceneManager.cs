using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PlayerCutsceneManager : MonoBehaviour
{
    public static PlayerCutsceneManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    [HideInInspector]
    public ThirdPersonCamera thirdPersonCam;
    [HideInInspector]
    public PlayerController playerController;

    public Transform camTransform;
    public float camRotSpeed;

    public Volume camVignette;
    public float camVignetteLoadTime;

    public TextMeshProUGUI camTextObj;
    public float showTextDelay;
    public float textShowUpTime;

    public Animator anim;

    public Vector3 finalCamTransformPos;
    public Quaternion finalCamTransformRot;
    public float smoothSpeed;

    private bool cutscene;
    public UnityEvent OnCutsceneFinished;

    public bool dead;


    private void Start()
    {
        thirdPersonCam = ThirdPersonCamera.Instance;
        playerController = PlayerController.Instance;
        anim = thirdPersonCam.GetComponentInParent<Animator>();
    }

    public void KillBySpikes(Vector3 spikePos, float moveSmooth, float rotSmooth)
    {
        playerController.canMove = false;
        cutscene = true;
        dead = true;
        anim.SetBool("disableCam", true);

        playerController.rb.velocity = new Vector3(playerController.rb.velocity.x / 1.5f, -1.5f, playerController.rb.velocity.z / 1.5f);
        playerController.rb.constraints = RigidbodyConstraints.None;

        thirdPersonCam.ChangeCamUpdateMode(false);
        thirdPersonCam.ChangeCamFollowTransform(camTransform);

        camTransform.SetPositionAndRotation(playerController.transform.position,
            thirdPersonCam.camRotPointX.rotation);

        finalCamTransformPos = spikePos + Vector3.up / 3;
        finalCamTransformRot = Quaternion.Euler(90, camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);

        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));

        StartCoroutine(CamText());
        StartCoroutine(RespawnPlayer(12));
    }
    public void PlayCutscene(Vector3 camPos, Quaternion camRot, float moveSmooth, float rotSmooth)
    {
        playerController.canMove = false;
        playerController.rb.velocity = Vector3.zero;
        cutscene = true;

        thirdPersonCam.ChangeCamUpdateMode(false);
        thirdPersonCam.ChangeCamFollowTransform(camTransform);

        camTransform.SetPositionAndRotation(playerController.transform.position,
            thirdPersonCam.camRotPointX.rotation);

        finalCamTransformPos = camPos;
        finalCamTransformRot = camRot;

        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));
    }
    public void ResetCamToPlayerView(float moveSmooth, float rotSmooth)
    {
        cutscene = true;

        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));

        finalCamTransformPos = thirdPersonCam.camRotPointX.position;
        finalCamTransformRot = thirdPersonCam.camRotPointX.rotation;
    }


    private IEnumerator UpdateCam(float smoothSpeed, float camRotSpeed)
    {
        while (cutscene)
        {
            yield return null;
            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, finalCamTransformRot, camRotSpeed * Time.deltaTime);

            camTransform.position = Vector3.MoveTowards(camTransform.position, finalCamTransformPos, smoothSpeed * Time.deltaTime);

            if (dead && camVignette.weight != 1)
            {
                camVignette.weight += 1 / camVignetteLoadTime * Time.deltaTime;
            }

            if (camTransform.position == finalCamTransformPos && camTransform.rotation == finalCamTransformRot)
            {
                cutscene = false;
                OnCutsceneFinished.Invoke();
                yield break;
            }
        }
    }


    private IEnumerator CamText()
    {
        yield return new WaitForSeconds(showTextDelay);

        while (camTextObj.color.a != 1)
        {
            yield return null;
            camTextObj.color += new Color(0, 0, 0, 1 / textShowUpTime * Time.deltaTime);
        }
    }
    private IEnumerator RespawnPlayer(float respawnDelay)
    {
        yield return new WaitForSeconds(respawnDelay);
        anim.SetBool("disableCam", false);

        playerController.rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerController.rb.rotation = Quaternion.identity;
    }
}