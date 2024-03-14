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



    private ThirdPersonCamera thirdPersonCam;
    [HideInInspector]
    public Rigidbody rb;

    public bool canMove;

    public Vector3 moveDir;
    public float moveSpeed;


    private void Start()
    {
        thirdPersonCam = GetComponent<ThirdPersonCamera>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector3>();
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
