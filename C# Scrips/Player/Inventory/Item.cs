using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemId;
    public int stackSize;
    public int amount;
    public TextMeshProUGUI amountText;

    public string itemName;

    public GameObject inGameObject;

    public Weapon weapon;
    public Armor armor;


    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshProUGUI>(true);
    }


    public void UpdateAmount(int newAmount)
    {
        amount = newAmount;

        if (amount < 2)
        {
            amountText.text = string.Empty;
            return;
        }
        amountText.text = amount.ToString();
    }
}
