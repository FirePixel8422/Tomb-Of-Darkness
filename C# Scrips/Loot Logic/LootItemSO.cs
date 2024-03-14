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
public struct Weapon
{
    public int t;
}
[System.Serializable]
public struct Armor
{
    public float damageReduction;
    public float speedModifier;
}
