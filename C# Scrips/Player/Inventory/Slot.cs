using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Inventory inventory;

    public Item heldItem;

    public bool full;
    public int slotId;

    public float dubbelClickTimer;
    private float timer;


    private void Start()
    {
        inventory = Inventory.Instance;
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    public void OnLeftClick()
    {
        if (inventory == null)
        {
            inventory = Inventory.Instance;
        }
        if (full && inventory.itemHeld == false)
        {
            //als dit slot wat heeft en de cursor niet
            // -pak item op met cursor

            timer = dubbelClickTimer;

            inventory.itemHeld = true;
            full = false;
            heldItem.transform.SetParent(transform.root, true);
            inventory.HeldItem = heldItem;
            heldItem = null;
        }
        else if (inventory.itemHeld == true)
        {
            //anders als de player met de cursor wel wat vasthoud
            if (full)
            {
                //als dit slot iets vasthoud
                if (heldItem.itemId == inventory.HeldItem.itemId && heldItem.amount != heldItem.stackSize && inventory.HeldItem.amount != heldItem.stackSize)
                {
                    //als het item in dit slot hetzelfde soort item is wat de player met de cursor vastheeft en het slotItem niet al vol zit (stackSize)
                    if ((heldItem.amount + inventory.HeldItem.amount) > heldItem.stackSize)
                    {
                        inventory.HeldItem.UpdateAmount(heldItem.stackSize - heldItem.amount);
                        heldItem.UpdateAmount(heldItem.stackSize);
                        return;
                    }
                    else
                    {
                        inventory.HeldItem.UpdateAmount(inventory.HeldItem.amount + heldItem.amount);
                        Destroy(heldItem.gameObject);
                    }
                }
                else
                {
                    //item in slot is anders dan item op cursor
                    Item temp = heldItem;

                    heldItem = inventory.HeldItem;
                    heldItem.transform.SetParent(transform, false, false);

                    inventory.HeldItem = temp;
                    return;
                }
            }
            inventory.itemHeld = false;
            full = true;
            heldItem = inventory.HeldItem;
            heldItem.transform.SetParent(transform, false, false);

            if (heldItem.amount != heldItem.stackSize && timer > 0)
            {
                inventory.StackAllItemToMax(heldItem);
            }
        }
    }
    public void OnRightClick()
    {
        if (inventory == null)
        {
            inventory = Inventory.Instance;
        }
        if (full)
        {
            //dit slot heeft iets vast
            if (inventory.itemHeld == false)
            {
                //player heeft niks vast met cursor
                if (heldItem.amount == 1)
                {
                    //slot heeft 1 van een item vast
                    // -pak item met cursor

                    full = false;
                    heldItem.transform.SetParent(transform.root, true);
                    inventory.HeldItem = heldItem;
                    inventory.itemHeld = true;
                    heldItem = null;
                }
                else
                {
                    //slot heeft meer dan 1 van een item vast
                    // -geef player cursor de helft (omlaag afgerond) van het item (via een kopie)

                    int amount = heldItem.amount / 2;
                    heldItem.UpdateAmount(heldItem.amount - amount);

                    Item copiedItem = Instantiate(heldItem.gameObject, transform, false).GetComponent<Item>();
                    copiedItem.UpdateAmount(amount);

                    inventory.itemHeld = true;
                    inventory.HeldItem = copiedItem;
                }
            }
            else if (heldItem.itemId == inventory.HeldItem.itemId && heldItem.stackSize != heldItem.amount)
            {
                //player heeft wel wat vast met cursor
                //en dat vastgehouden item is hetzelfde als het item in dit slot
                // -plaats 1 van item in slot

                heldItem.UpdateAmount(heldItem.amount + 1);
                inventory.HeldItem.UpdateAmount(inventory.HeldItem.amount - 1);

                if (inventory.HeldItem.amount == 0)
                {
                    // -vernietig item sprite als laatste kopie geplaatst is in slot
                    Destroy(inventory.HeldItem.gameObject);
                    inventory.itemHeld = false;
                }
            }
        }
        else if (inventory.itemHeld == true)
        {
            //dit slot heeft niks vast
            if (inventory.HeldItem.amount == 1)
            {
                //de player heeft 1 van een item vast met de cursor
                // -plaats item in slot.
                full = true;
                inventory.itemHeld = false;

                heldItem = inventory.HeldItem;
                heldItem.transform.SetParent(transform, false, false);
            }
            else
            {
                //de player heeft meer van 1 item vast met de cursor.
                // -plaats 1 kopie van cursor item naar slot

                Item copiedItem = Instantiate(inventory.HeldItem.gameObject, transform, false).GetComponent<Item>();
                heldItem = copiedItem;

                copiedItem.transform.SetParent(transform, false, false);
                copiedItem.UpdateAmount(1);

                inventory.HeldItem.UpdateAmount(inventory.HeldItem.amount - 1);
                full = true;
            }
        }
    }

    
}
