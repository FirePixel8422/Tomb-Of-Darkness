using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
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
            anim.SetTrigger("Open");
            canInteract = false;
            usable = false;
        }
    }
}