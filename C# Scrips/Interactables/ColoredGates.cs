using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredGates : Interactable
{
    public int id;
    public Animator anim;

    public override void Interact()
    {
        print("d");
        foreach (Slot slot in Inventory.Instance.slots)
        {
            if (slot.heldItem != null && slot.heldItem.itemId == id)
            {                
                Destroy(slot.heldItem.gameObject);
                slot.full = false;
                slot.heldItem = null;
                anim.SetBool("Open", true);
            }
        }
        Hotbar h =  FindObjectOfType<Hotbar>();
        foreach (Slot slot in h.GetComponentsInChildren<Slot>())
        {
            if (slot.heldItem != null && slot.heldItem.itemId == id)
            {
                Destroy(slot.heldItem.gameObject);
                slot.full = false;
                slot.heldItem = null;
                anim.SetBool("Open", true);
            }
        }
    }
}
