using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deafault LootItem", menuName = "LootLogic/LootItem")]
[System.Serializable]
public class LootItemSO : ScriptableObject
{
    public string itemName;

    public Sprite sprite;
    public GameObject inGameObject;

    public Weapon weapon;
    public Armor armor;
}
