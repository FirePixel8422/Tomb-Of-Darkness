using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredGates : Interactable
{
    public int id;

    public override void Interact()
    {
        foreach (Slot slot in Inventory.Instance.slots)
        {
            if (slot.heldItem != null && slot.heldItem.itemId == id)
            {
                Destroy(slot.heldItem.gameObject);
                slot.full = false;
            }
        }
    }
}
