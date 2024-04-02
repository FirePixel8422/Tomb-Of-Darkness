using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable 
{
    public Gate gate;

    private Animator anim;

    public float interactDelay;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (canInteract && usable)
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
