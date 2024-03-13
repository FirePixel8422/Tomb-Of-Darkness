using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    public float sensitivity = 2.0f; // Mouse sensitivity

    private float rotationX = 0;

    void Update()
    {
        // Capture mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotate the player horizontally based on mouse input
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically based on mouse input
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Limit vertical rotation to prevent flipping

        // Apply rotation to the camera
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);


     
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor

       
        
    }
}

