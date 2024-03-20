using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Slot[] slots;

    private Item heldItem;
    public Item HeldItem
    {
        get
        {
            return heldItem;
        }
        set
        {
            heldItem = value;
            heldItem.transform.SetParent(transform, true, false);
            UpdateMoveItemToMouse();
        }
    }
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
            UpdateMoveItemToMouse();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            FindObjectOfType<GraphicRaycaster>().Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                Slot slot = result.gameObject.GetComponent<Slot>();
                if (slot != null)
                {
                    slot.OnLeftClick();
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            FindObjectOfType<GraphicRaycaster>().Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                Slot slot = result.gameObject.GetComponent<Slot>();
                if (slot != null)
                {
                    slot.OnRightClick();
                }
            }
        }
    }

    public void UpdateMoveItemToMouse()
    {
        HeldItem.transform.position = Input.mousePosition;
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