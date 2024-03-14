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

    public Transform rotPoint;
    public float rotSpeed;

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


    private void Update()
    {
        if (canMove == false)
        {
            return;
        }
        if (moveDir == Vector3.zero)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        Quaternion rot = Quaternion.LookRotation(thirdPersonCam.camRotPointY.TransformDirection(moveDir));

        rotPoint.rotation = Quaternion.Lerp(rotPoint.rotation, rot, rotSpeed * Time.deltaTime);

        Vector3 forwardMoveDir = thirdPersonCam.camRotPointY.TransformDirection(
            new Vector3(moveDir.x, 0, moveDir.z));


        rb.velocity = new Vector3(forwardMoveDir.x * moveSpeed, rb.velocity.y, forwardMoveDir.z * moveSpeed);
    }
}
