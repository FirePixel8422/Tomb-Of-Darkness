using System.Collections;
using System.Collections.Generic;
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


    private void FixedUpdate()
    {
        rb.velocity = thirdPersonCam.camRotPointY.TransformDirection(new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed));
    }

}
