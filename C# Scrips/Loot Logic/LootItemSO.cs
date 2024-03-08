using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deafault LootItem", menuName = "LootLogic/LootItem")]
[System.Serializable]
public class LootItemSO : ScriptableObject
{
    public string itemName;

    public Weapon weapon;
    public Weapon armor;
}

[System.Serializable]
public class Weapon
{
    public int t;
}
[System.Serializable]
public class Armor
{
    public int t;
}
