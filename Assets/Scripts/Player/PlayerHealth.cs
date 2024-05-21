/*
 * File: PlayerHealth.cs
 * Purpose: Manages player health values throughout gameplay
 * Author: Devon
 * 
 * How to use:
 * In the clothing pickup script (when finished): 
 *     - Need to update the related clothing bool to true and call CheckHealth()
 *           e.g PlayerHealth.hatWorn = true;
 *               PlayerHealth.CheckHealth();
 * When there is a way to remove items of clothing, the switch statement in Damage() needs to be updated.
 * 
 * Notes:
 *     - I have currently left CheckHealth() running in Update() as we haven't added clothing pickups yet.
 *       It would be good to move this out of update ASAP as it could affect performance
 */

using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Clothing bools")] //Public so it can be modified by ClothingPickup script
    [SerializeField] public bool hatWorn;
    [SerializeField] public bool shirtWorn;
    [SerializeField] public bool pantsWorn;

    [Header("Health Variables")]
    private const int startingHP = 1;
    private int currentHP;

    /// <summary>
    /// Set the initial values of all variables
    /// </summary>
    void Start()
    {
        SetValuesToDefaults();
    }

    /// <summary>
    /// Updates the players health depending on currently 'worn' clothing items
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
    /// Sets the players health depending on currently 'worn' clothing items
    /// </summary>
    private void CheckHealth()
    {
        int healthIncrease = 0; //Value increases for each worn item of clothing

        if (hatWorn) { healthIncrease++; }
        if (shirtWorn) { healthIncrease++; }
        if (pantsWorn) { healthIncrease++;  }
        
        currentHP = startingHP + healthIncrease; //Limit health to a max of 4
        Debug.Log($"Current Health: {currentHP}"); //Show health value during gameplay as no visual representation currently
    }

    /// <summary>
    /// Set a random clothing item back to false to damage the player
    /// </summary>
    // Used in CrowCollision.cs
    // Need to modify later after Liam's clothing issue is implemented
    public void Damage()
    {
        // List of worn clothing items
        List<string> wornItems = new List<string>(); 
        if (hatWorn) wornItems.Add("hat");
        if (shirtWorn) wornItems.Add("shirt");
        if (pantsWorn) wornItems.Add("pants");

        if (wornItems.Count == 0)
        {
            GameManager.instance.gameoverEvent.Invoke();
        }
        else
        {
            // Randomly select an item to remove from list of worn items
            int index = Random.Range(0, wornItems.Count);
            string itemToRemove = wornItems[index];

            switch (itemToRemove)
            {
                case "hat":
                    hatWorn = false; //Remove item of clothing from health calculation
                    //Additional code to remove hat from player here
                    break;
                case "shirt":
                    shirtWorn = false;
                    //Additional code to remove shirt from player here
                    break;
                case "pants":
                    pantsWorn = false;
                    //Additional code to remove pants from player here
                    break;
            }
        }
        CheckHealth(); //Ensure current health is correct after removing items
    }
}
