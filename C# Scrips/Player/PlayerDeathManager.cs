using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDeathManager : MonoBehaviour
{
    public static PlayerDeathManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    [HideInInspector]
    public ThirdPersonCamera thirdPersonCam;
    [HideInInspector]
    public PlayerController playerController;
    private Rigidbody rb;

    public Transform deathTransform;
    public float deathRotSpeed;

    public Volume deathVignette;
    public float deathVignetteLoadTime;

    public TextMeshProUGUI deathTextObj;
    public float showTextDelay;
    public float textShowUpTime;

    public Animator anim;

    public Vector3 finalDeathTransformPos;
    public Quaternion finalDeathTransformRot;
    public float smoothSpeed;

    private bool cutscene;
    public bool dead;


    private void Start()
    {
        thirdPersonCam = ThirdPersonCamera.Instance;
        playerController = PlayerController.Instance;
        rb = playerController.GetComponent<Rigidbody>();
        anim = thirdPersonCam.GetComponentInParent<Animator>();
    }

    public void KillBySpikes(Vector3 spikePos, float moveSmooth, float rotSmooth)
    {
        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));

        playerController.canMove = false;
        cutscene = true;
        dead = true;
        anim.SetBool("disableCam", true);

        rb.velocity = new Vector3(rb.velocity.x / 1.5f, -1.5f, rb.velocity.z / 1.5f);
        rb.constraints = RigidbodyConstraints.None;

        deathTransform.SetPositionAndRotation(playerController.transform.position,
            thirdPersonCam.camRotPointX.rotation);

        finalDeathTransformPos = spikePos + Vector3.up / 3;
        finalDeathTransformRot = Quaternion.Euler(90, deathTransform.localEulerAngles.y, deathTransform.localEulerAngles.z);

        thirdPersonCam.ChangeCamFollowTransform(deathTransform);

        StartCoroutine(DeathText());
        StartCoroutine(RespawnPlayer(8));
    }
    public void PlayCutscene(Vector3 camPos, Quaternion camRot, float moveSmooth, float rotSmooth)
    {
        playerController.canMove = false;
        playerController.rb.velocity = Vector3.zero;
        cutscene = true;

        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));

        thirdPersonCam.ChangeCamFollowTransform(deathTransform);

        deathTransform.SetPositionAndRotation(playerController.transform.position,
            thirdPersonCam.camRotPointX.rotation);

        finalDeathTransformPos = camPos;
        finalDeathTransformRot = camRot;

        StartCoroutine(EndCutSceneDelay(5));
    }
    public void ResetCamToPlayerView(float moveSmooth, float rotSmooth)
    {
        cutscene = true;
        playerController.canMove = true;

        StartCoroutine(UpdateCam(moveSmooth, rotSmooth));

        finalDeathTransformPos = thirdPersonCam.camRotPointX.position;
        finalDeathTransformRot = thirdPersonCam.camRotPointX.rotation;
    }
    private IEnumerator EndCutSceneDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cutscene = false;
    }


    private IEnumerator UpdateCam(float smoothSpeed, float deathRotSpeed)
    {
        while (cutscene)
        {
            yield return null;

            deathTransform.rotation = Quaternion.Lerp(deathTransform.rotation, finalDeathTransformRot, deathRotSpeed * Time.deltaTime);

            deathTransform.position = Vector3.MoveTowards(deathTransform.position, finalDeathTransformPos, smoothSpeed * Time.deltaTime);

            if (dead)
            {
                deathVignette.weight += 1 / deathVignetteLoadTime * Time.deltaTime;
            }

            if (Vector3.Distance(deathTransform.position, finalDeathTransformPos) < 0.5f && Quaternion.Angle(deathTransform.rotation, finalDeathTransformRot) < 0.1f)
            {
                cutscene = false;
                yield break;
            }
        }
    }


    private IEnumerator DeathText()
    {
        yield return new WaitForSeconds(showTextDelay);

        while (deathTextObj.color.a != 1)
        {
            yield return null;
            deathTextObj.color += new Color(0, 0, 0, 1 / textShowUpTime * Time.deltaTime);
        }
    }
    private IEnumerator RespawnPlayer(float respawnDelay)
    {

        yield return new WaitForSeconds(respawnDelay);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.rotation = Quaternion.identity;
    }
}