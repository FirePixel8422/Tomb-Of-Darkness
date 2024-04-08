using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLogicManager : MonoBehaviour
{
    public static LootLogicManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Color[] rarityColors;
}


[System.Serializable]
public class LootItemRarity
{
    public float rarityChance;
    public ItemRarity rarity;

    public LootItem[] lootItems;
}

[System.Serializable]
public class LootItem
{
    public LootItemSO lootItem;
    public int minAmount = 1, maxAmount = 1;
}
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    VeryRare,
    Epic,
    Legendary,
    Mythic,
    Ancient,
    God,
};