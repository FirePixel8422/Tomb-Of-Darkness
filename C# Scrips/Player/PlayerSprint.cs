using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{

    public PlayerController pController;

    public float sprintMultiplier;
    public bool isSprinting;
    public bool sprintHeld;

    public float totalSprintTime;
    public float currentSprintTime;

    public float fullSprintRegainTime;
    public float minPercentageSprintToSprint;

    private float minRequiredSprintToSprint;


    private void Start()
    {
        pController = GetComponent<PlayerController>();
        minRequiredSprintToSprint = totalSprintTime / 100 * minPercentageSprintToSprint;
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintHeld = ctx.ReadValueAsButton();
    }

    private void Update()
    {
        if (sprintHeld && currentSprintTime > minRequiredSprintToSprint && isSprinting == false)
        {
            isSprinting = true;
            pController.moveSpeed *= sprintMultiplier;
        }
        if (sprintHeld == false && isSprinting)
        {
            isSprinting = false;
            pController.moveSpeed /= sprintMultiplier;
        }

        if (isSprinting)
        {
            currentSprintTime -= Time.deltaTime;
            if(currentSprintTime < 0)
            {
                isSprinting = false;
                pController.moveSpeed /= sprintMultiplier;
                currentSprintTime = 0;
            }
        }
        else if(currentSprintTime < totalSprintTime)
        {
            currentSprintTime += 1 / fullSprintRegainTime * Time.deltaTime;

            if (currentSprintTime > totalSprintTime)
            {
                currentSprintTime = totalSprintTime;
            }
        }
    }
}
