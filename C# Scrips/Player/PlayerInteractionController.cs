using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    public LayerMask interactablesLayers;
    public Transform interactionRaycastTransform;
    public float interactionRange;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Physics.Raycast(interactionRaycastTransform.position, interactionRaycastTransform.forward, out RaycastHit hit, interactionRange, interactablesLayers, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.TryGetComponent(out Interactable interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
