using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsHandler : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public float tempHealth;

    public bool alive;

    public Armor armor;

    public PlayerController pController;
    public PlayerSprint pSprint;

    private void Start()
    {
        pController = GetComponent<PlayerController>();
        pSprint = GetComponent<PlayerSprint>();
    }


    public void TakeDamage(float damage, float ignoreArmorPercentage)
    {
        float finalDamage = damage / 100 * (100 - Mathf.Clamp(armor.damageReduction - ignoreArmorPercentage, 0, 100));

        tempHealth -= finalDamage;

        if (tempHealth < 0)
        {
            health += tempHealth;
            if(health < 0)
            {
                alive = false;
            }
        }
    }
}
