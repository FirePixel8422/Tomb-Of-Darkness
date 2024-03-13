using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private void Awake()
    {
        Instance = this; 
    }

    public ThirdPersonCamera thirdPersonCam;
    public Transform deathTransform;
    public float deathRotSpeed;

    public bool canMove;

    private Rigidbody rb;
    public Vector3 moveDir;
    public float moveSpeed;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        thirdPersonCam = GetComponent<ThirdPersonCamera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector3>();
    }

    public void Falling(bool state)
    {
        if (state == true)
        {
            canMove = false;
            rb.velocity = new Vector3(rb.velocity.x / 1.5f, -1.5f, rb.velocity.z / 1.5f);
            rb.constraints = RigidbodyConstraints.None;
            deathTransform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            deathTransform.rotation = thirdPersonCam.camRotPointY.localRotation;
            thirdPersonCam.camTargetGroup.m_Targets[0].target = deathTransform;
        }
        else
        {
            canMove = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            deathTransform.localEulerAngles = new Vector3(Mathf.MoveTowards(deathTransform.localEulerAngles.x, 90, deathRotSpeed * Time.deltaTime),
                deathTransform.localEulerAngles.y,
                Mathf.MoveTowards(deathTransform.localEulerAngles.z, 0, deathRotSpeed * Time.deltaTime));
        }
    }


    private void FixedUpdate()
    {
        if(canMove == false)
        {
            return;
        }
        rb.velocity = thirdPersonCam.camRotPointY.TransformDirection(new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed));
    }

}
