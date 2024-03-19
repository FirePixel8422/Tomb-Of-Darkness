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


    private void Start()
    {
        amountText = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void UpdateAmountText(int newAmount)
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
