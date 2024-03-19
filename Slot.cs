using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Inventory inventory;

    public Transform itemHolder;

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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }



    public void OnLeftClick()
    {
        GetComponent<Image>().color = Color.white;

        if (full && inventory.itemHeld == false)
        {
            //als dit slot wat heeft en de cursor niet
            // -pak item op met cursor

            timer = dubbelClickTimer;

            inventory.itemHeld = true;
            full = false;
            heldItem.transform.SetParent(transform.root, true);
            inventory.heldItem = heldItem;
            heldItem = null;
        }
        else if (inventory.itemHeld == true)
        {
            //anders als de player met de cursor wel wat vasthoud
            if (full)
            {
                //als dit slot iets vasthoud
                if (heldItem.itemId == inventory.heldItem.itemId && heldItem.amount != heldItem.stackSize)
                {
                    //als het item in dit slot hetzelfde soort item is wat de player met de cursor vastheeft en het slotItem niet al vol zit (stackSize)
                    if (heldItem.amount + inventory.heldItem.amount > heldItem.stackSize)
                    {
                        inventory.heldItem.UpdateAmountText(heldItem.stackSize - heldItem.amount);
                        heldItem.UpdateAmountText(heldItem.stackSize);
                    }
                    else
                    {
                        inventory.heldItem.UpdateAmountText(inventory.heldItem.amount + heldItem.amount);
                        Destroy(heldItem.gameObject);
                    }
                }
                else
                {
                    Item temp = heldItem;

                    heldItem = inventory.heldItem;
                    heldItem.transform.SetParent(itemHolder, false, false);

                    inventory.heldItem = temp;
                    inventory.heldItem.transform.SetParent(transform.root);
                    return;
                }
            }
            inventory.itemHeld = false;
            full = true;
            heldItem = inventory.heldItem;
            heldItem.transform.SetParent(itemHolder, false, false);

            if (heldItem.amount != heldItem.stackSize && timer > 0)
            {
                inventory.StackAllItemToMax(heldItem);
            }
        }
    }
    public void OnRightClick()
    {
        GetComponent<Image>().color = Color.black;
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

                    inventory.itemHeld = true;
                    full = false;
                    heldItem.transform.SetParent(transform.root, true);
                    inventory.heldItem = heldItem;
                    heldItem = null;
                }
                else
                {
                    //slot heeft meer dan 1 van een item vast
                    // -geef player cursor de helft (omlaag afgerond) van het item (via een kopie)

                    int amount = heldItem.amount / 2;
                    heldItem.UpdateAmountText(heldItem.amount - amount);

                    Item copiedItem = Instantiate(heldItem.gameObject, transform.root).GetComponent<Item>();
                    copiedItem.UpdateAmountText(amount);

                    inventory.itemHeld = true;
                    inventory.heldItem = copiedItem;
                }
            }
            else if (heldItem.itemId == inventory.heldItem.itemId && heldItem.stackSize != heldItem.amount)
            {
                //player heeft wel wat vast met cursor
                //en dat vastgehouden item is hetzelfde als het item in dit slot
                // -plaats 1 van item in slot

                heldItem.UpdateAmountText(heldItem.amount + 1);
                inventory.heldItem.UpdateAmountText(inventory.heldItem.amount - 1);

                if (inventory.heldItem.amount == 0)
                {
                    // -vernietig item sprite als laatste kopie geplaatst is in slot
                    Destroy(inventory.heldItem.gameObject);
                    inventory.itemHeld = false;
                }
            }
        }
        else if(inventory.itemHeld == true)
        {
            //dit slot heeft niks vast
            if (inventory.heldItem.amount == 1)
            {
                //de player heeft 1 van een item vast met de cursor
                // -plaats item in slot.
                full = true;
                inventory.itemHeld = false;

                heldItem = inventory.heldItem;
                heldItem.transform.SetParent(itemHolder, false);
                heldItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                //de player heeft meer van 1 item vast met de cursor.
                // -plaats 1 kopie van cursor item naar slot

                Item copiedItem = Instantiate(inventory.heldItem.gameObject).GetComponent<Item>();
                heldItem = copiedItem;

                copiedItem.transform.SetParent(itemHolder, false, false);
                copiedItem.UpdateAmountText(1);

                inventory.heldItem.UpdateAmountText(inventory.heldItem.amount - 1);
                full = true;
            }
        }
    }

    
}
