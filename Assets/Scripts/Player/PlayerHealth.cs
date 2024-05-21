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

    [Header("Player Clothing bools")] //Public so it can be modified by ClothingPickup script
    public bool hatWorn;
    public bool shirtWorn;
    public bool pantsWorn;

    /// <summary>
    /// Set the initial values of all variables
    /// </summary>
    void Start()
    {
        SetValuesToDefaults();
    }

    /// <summary>
    /// Update the player health value during gameplay
    /// </summary>
    private void Update()
    {
        CheckHealth();
    }

    /// <summary>
    /// Set the initial values of all variables in Start and Reset
    /// </summary>
    public void SetValuesToDefaults()
    {
        currentHP = startingHP; //Set the health back to 1
        hatWorn = false;
        shirtWorn = false;
        pantsWorn = false;
    }


    /// <summary>
    /// Updates health during gameplay depending on clothes worn 
    /// </summary>
    void CheckHealth()
    {
        int healthIncrease = 0; //Value increases for each worn item of clothing

        if (hatWorn) { healthIncrease++; }
        if (shirtWorn) { healthIncrease++; }
        if (pantsWorn) { healthIncrease++;  }
        
        currentHP = startingHP + healthIncrease; //Limit health to a max of 4
    }
}
