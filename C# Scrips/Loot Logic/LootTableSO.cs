using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Deafault LootTable", menuName = "LootLogic/LootTable")]
public class LootTableSO : ScriptableObject
{
    public LootItemRarity[] lootItemRarity;
}