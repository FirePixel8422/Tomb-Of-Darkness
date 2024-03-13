using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance;
    private void Awake()
    {
        Instance = this;
    }


    public CinemachineTargetGroup camTargetGroup;
    public CinemachineVirtualCamera cam;
    public bool camInput = true;

    public float xSens, ySens;

    public Transform camRotPointY;
    public Transform camRotPointX;
    public float clampHorizontal;

    public Vector2 input;


    public void OnMoveCamera(InputAction.CallbackContext ctx)
    {
        Vector2 vec = ctx.ReadValue<Vector2>();
        input = new Vector2(vec.x * xSens, -vec.y * ySens);
    }

    public void ChangeCamUpdateMode(bool state)
    {
        camInput = state;
    }

    private void Update()
    {
        if(camInput == false)
        {
            return;
        }

        Vector3 camRot = camRotPointY.localEulerAngles + camRotPointX.localEulerAngles;
        camRot.x += input.y;
        camRot.y += input.x;

        if (camRot.x > 180f)
        {
            camRot.x -= 360f;
        }

        camRotPointY.Rotate(new Vector3(input.y, 0, 0));
        camRotPointY.localRotation = Quaternion.Euler(0, camRot.y, 0);

        camRotPointX.Rotate(new Vector3(0, input.x, 0));
        camRotPointX.localRotation = Quaternion.Euler(Mathf.Clamp(camRot.x, -clampHorizontal, clampHorizontal), 0, 0);
    }
}
