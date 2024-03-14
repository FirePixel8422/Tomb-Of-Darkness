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

    public Vector3 finalDeathTransformPos;
    public float smoothSpeed;


    private void Start()
    {
        thirdPersonCam = ThirdPersonCamera.Instance;
        playerController = PlayerController.Instance;
        rb = playerController.GetComponent<Rigidbody>();
    }

    public void KillBySpikes(Vector3 spikePos)
    {
        playerController.canMove = false;

        rb.velocity = new Vector3(rb.velocity.x / 1.5f, -1.5f, rb.velocity.z / 1.5f);
        rb.constraints = RigidbodyConstraints.None;

        deathTransform.SetPositionAndRotation(playerController.transform.position,
            thirdPersonCam.camRotPointX.localRotation);

        finalDeathTransformPos = spikePos;

        thirdPersonCam.camTargetGroup.m_Targets[0].target = deathTransform;

        StartCoroutine(DeathText());
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

    private void Update()
    {
        if (playerController.canMove == false)
        {
            deathTransform.localEulerAngles = new Vector3(Mathf.MoveTowards(deathTransform.localEulerAngles.x, 90, deathRotSpeed * Time.deltaTime),
                deathTransform.localEulerAngles.y,
                deathTransform.localEulerAngles.z);

            deathTransform.position = Vector3.MoveTowards(deathTransform.position, finalDeathTransformPos, smoothSpeed * Time.deltaTime);

            deathVignette.weight += 1 / deathVignetteLoadTime * Time.deltaTime;
        }
    }

    public IEnumerator RespawnPlayer(float respawnDelay)
    {

        yield return new WaitForSeconds(respawnDelay);

        playerController.canMove = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}