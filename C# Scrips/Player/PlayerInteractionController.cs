using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private PlayerController pController;

    public LayerMask interactablesLayers;
    public Transform interactionRaycastTransform;
    public float interactionRange;

    private void Start()
    {
        pController = PlayerController.Instance;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed &&pController.canMove)
        {
            if (Physics.Raycast(interactionRaycastTransform.position, interactionRaycastTransform.forward, out RaycastHit hit, interactionRange, interactablesLayers, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.isTrigger && hit.collider.TryGetComponent(out Interactable interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
