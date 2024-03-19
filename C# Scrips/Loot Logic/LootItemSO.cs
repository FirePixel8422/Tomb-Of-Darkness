using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deafault LootItem", menuName = "LootLogic/LootItem")]
[System.Serializable]
public class LootItemSO : ScriptableObject
{
    public string itemName;

    public Weapon weapon;
    public Armor armor;
}

[System.Serializable]
public struct Weapon
{
    public float damage;
    public float attackSpeed;
    public float reach;
}
[System.Serializable]
public struct Armor
{
    public float damageReduction;
    public float speedModifier;
}
