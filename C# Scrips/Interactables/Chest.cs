using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : Interactable
{
    public LootTableSO[] loot;
    public List<Item> items = new List<Item>();

    public GameObject chestUI;
    private Animator anim;

    public Slot[] slots;

    public float interactRange;

    public bool lootSpawned;
    public bool open;


    private void Start()
    {
        anim = GetComponent<Animator>();
        slots = GetComponentsInChildren<Slot>(true);

        CanvasManager.AddUIToCanvas(chestUI.transform);
    }

    public override void Interact()
    {
        if(Vector3.Distance(PlayerController.Instance.transform.position, transform.position) > interactRange)
        {
            return;
        }

        anim.SetTrigger("Open");
        if (lootSpawned == false)
        {
            lootSpawned = true;
            int currentSlot = 0;
            foreach (LootTableSO lootTable in loot)
            {
                float r = Random.Range(0, 100);
                foreach (LootItemRarity rarity in lootTable.lootItemRarity)
                {
                    if (r > rarity.rarityChance)
                    {
                        r -= rarity.rarityChance;
                    }
                    else
                    {
                        r = Random.Range(0, 100);
                        for (int i = 0; i < rarity.lootItems.Length; i++)
                        {
                            if (r > (100 / rarity.lootItems.Length * (i + 1)))
                            {
                                r -= 100 / rarity.lootItems.Length;
                            }
                            else
                            {
                                Item itemObj = Instantiate(rarity.lootItems[i].lootItem, slots[currentSlot].transform, false).GetComponent<Item>();
                                items.Add(itemObj);

                                items[items.Count - 1].UpdateAmount(Random.Range(rarity.lootItems[i].minAmount, rarity.lootItems[i].maxAmount + 1));
                                slots[currentSlot].heldItem = itemObj;
                                slots[currentSlot].full = true;
                                break;
                            }
                        }
                        break;
                    }
                }
                currentSlot += 1;
            }
        }
        open = !open;
        chestUI.SetActive(open);
        if (open)
        {
            PlayerController.Instance.OpenInventory();
        }
        else
        {
            PlayerController.Instance.CloseInventory();
        }
    }

    private void Update()
    {
        if (open)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F))
            {
                open = false;
                anim.SetTrigger("Open");

                PlayerController.Instance.CloseInventory();

                chestUI.SetActive(false);
            }
            else if(Vector3.Distance(PlayerController.Instance.transform.position, transform.position) > interactRange)
            {
                open = false;
                anim.SetTrigger("Open");

                PlayerController.Instance.CloseInventory();

                chestUI.SetActive(false);
            }
        }
    }
}
