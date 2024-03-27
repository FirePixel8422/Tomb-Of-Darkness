using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable 
{
    public Gate gate;

    private Animator anim;
    private PlayerController pController;

    public float interactDelay;
    

    private void Start()
    {
        pController = PlayerController.Instance;
        anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (canInteract && pController.canMove && usable)
        {
            StartCoroutine(PerformInteraction());
        }
    }

    private IEnumerator PerformInteraction()
    {
        anim.SetTrigger("Pull");
        canInteract = false;
        usable = false;

        yield return new WaitForSeconds(interactDelay);

        gate.StartCoroutine(gate.OpenGate());

        canInteract = true;
    }
}
