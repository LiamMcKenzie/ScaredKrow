/*
 * File: PlayerHealth.cs
 * Purpose: Manages player health values throughout gameplay
 * Author: Devon
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private const int startingHP = 1;
    private int currentHP;

    [Header("Player Clothing bools")]
    public bool hatWorn;
    public bool shirtWorn;
    public bool pantsWorn;

    void Start()
    {
        SetValuesToDefaults();
    }
    
    void SetValuesToDefaults()
    {
        currentHP = startingHP; //Set the health back to 1
        hatWorn = false;
        shirtWorn = false;
        pantsWorn = false;
    }

    private void Update()
    {
        CheckHealth();
    }

    void CheckHealth()
    {
        int healthIncrease = 0; //Value increases for each worn item of clothing

        if (hatWorn) { healthIncrease++; }
        if (shirtWorn) { healthIncrease++; }
        if (pantsWorn) { healthIncrease++;  }
        
        currentHP = startingHP + healthIncrease;
        Debug.Log($"Current Health: {currentHP}");
        Debug.Log($"Health Increase: {healthIncrease}");
    }
}
