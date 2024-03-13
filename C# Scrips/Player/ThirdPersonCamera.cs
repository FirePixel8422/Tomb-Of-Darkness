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
    public bool updateCam = true;

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
        updateCam = state;
        camTargetGroup.m_Targets[0].weight = state == false ? 0 : 1;
    }

    private void Update()
    {
        if(updateCam == false)
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
