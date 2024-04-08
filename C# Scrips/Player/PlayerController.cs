using System.Linq;
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

    public Inventory inventory;
    public Transform hotBar;

    public bool canMove;

    public Transform rotPoint;
    public float rotSpeed;

    public Vector3 moveDir;
    public float moveSpeed;

    public float jumpForce;
    public Transform groundCheck;
    public LayerMask groundLayer;


    private void Start()
    {
        thirdPersonCam = GetComponent<ThirdPersonCamera>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        CanvasManager.AddUIToCanvas(inventory.transform);
        CanvasManager.AddUIToCanvas(hotBar);
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector3>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Physics.OverlapSphere(groundCheck.position, .1f, groundLayer).Length != 0 && canMove)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void OnOpenCloseInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (inventory.gameObject.activeInHierarchy)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    public void OpenInventory()
    {
        inventory.gameObject.SetActive(true);
        thirdPersonCam.camInput = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseInventory()
    {
        inventory.gameObject.SetActive(false);
        thirdPersonCam.camInput = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory.SaveInventory();
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
