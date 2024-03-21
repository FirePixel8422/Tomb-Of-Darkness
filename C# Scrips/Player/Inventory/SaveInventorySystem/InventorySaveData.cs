using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public SlotData[] slotData;

    public InventorySaveData(InventorySaveLoadFunctions p)
    {
        slotData = p.slotData;
    }
}


[System.Serializable]
public struct SlotData
{
    public bool full;
    public int itemId;
    public int itemAmount;

    public void SetData(bool _full, int _itemId, int _itemAmount)
    {
        full = _full;
        itemId = _itemId;
        itemAmount = _itemAmount;
    }
}
