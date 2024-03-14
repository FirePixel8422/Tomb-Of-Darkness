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
    public float accelSpeed;
    public Vector3 moveCap;


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


    private void Update()
    {
        if (canMove == false)
        {
            return;
        }
        rb.velocity += (thirdPersonCam.camRotPointY.TransformDirection(
            new Vector3(moveDir.x * accelSpeed, 0, moveDir.z * accelSpeed) * Time.deltaTime));

        print(thirdPersonCam.camRotPointY.TransformDirection(
            new Vector3(moveDir.x * accelSpeed, 0, moveDir.z * accelSpeed) * Time.deltaTime));
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveCap.x, moveCap.x), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveCap.z, moveCap.z));
    }
}
