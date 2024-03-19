using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Slot[] slots;

    public Item heldItem;
    public bool itemHeld;


    private void Start()
    {
        slots = GetComponentsInChildren<Slot>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotId = i;
        }
    }

    private void Update()
    {
        if (itemHeld)
        {
            heldItem.transform.position = Input.mousePosition;
        }
    }

    public void StackAllItemToMax(Item item)
    {
        int amount = item.amount;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].full && slots[i].heldItem.itemId == item.itemId)
            {
                if (slots[i].heldItem == item)
                {
                    continue;
                }
                if ((amount + slots[i].heldItem.amount) > item.stackSize)
                {
                    slots[i].heldItem.UpdateAmountText(slots[i].heldItem.amount - (item.stackSize - amount));
                    amount = item.stackSize;
                    break;
                }
                else
                {
                    amount += slots[i].heldItem.amount;
                    Destroy(slots[i].heldItem.gameObject);
                    slots[i].full = false;
                }
            }
        }
        item.UpdateAmountText(amount);
    }
}
