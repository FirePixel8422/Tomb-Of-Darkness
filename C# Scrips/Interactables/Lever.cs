using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable 
{
    public Gate[] gate;

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
            anim.SetTrigger("Pull");
            canInteract = false;
            usable = false;

            MusicManager.Instance.ChangeMusicTrack(0, 0.2f);

            StartCoroutine(PerformInteraction());
        }
    }

    private IEnumerator PerformInteraction()
    {
        yield return new WaitForSeconds(interactDelay);

        foreach (Gate gate in gate)
        {
            yield return null;
            gate.StartCoroutine(gate.OpenGate());
        }
        canInteract = true;
    }
}
