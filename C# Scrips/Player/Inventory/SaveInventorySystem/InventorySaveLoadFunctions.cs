using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySaveLoadFunctions : MonoBehaviour
{
    public Item[] allItemsOfGame;

    public Inventory inv;
    public SlotData[] slotData;


    public void Init()
    {
        for (int i = 0; i < allItemsOfGame.Length; i++)
        {
            allItemsOfGame[i].itemId = i;
        }

        inv = Inventory.Instance;

        InventorySaveData data = SaveAndLoadInventory.LoadInfo();
        if (data != null)
        {
            //LoadInventoryFromFile(data);
        }
    }


    public void LoadInventoryFromFile(InventorySaveData data)
    {
        inv.ClearInventory();
        slotData = data.slotData;
        for (int i = 0; i < slotData.Length; i++)
        {
            if (slotData[i].full)
            {
                Slot slot = inv.slots[i];

                slot.heldItem = Instantiate(allItemsOfGame[slotData[i].itemId], transform, false);
                slot.heldItem.transform.SetParent(slot.transform, false, false);
                
                slot.full = true;
                slot.heldItem.UpdateAmount(slotData[i].itemAmount);
            }
        }
    }
}
