/*
 * File: PlayerHealth.cs
 * Purpose: Manages the player's health and clothing
 * Author: Johnathan and Devon
 * Contributions: Assisted by GitHub Copilot
 */

using System.Collections.Generic;
using UnityEngine;

public enum ClothingType
{
    Hat,
    Shirt,
    Pants
}

public class ClothingItem
{
    public ClothingType type;
    public bool worn;
}

public class OutfitController2 : MonoBehaviour
{
    private List<ClothingItem> clothingItems = new List<ClothingItem>() {
        new ClothingItem { type = ClothingType.Hat, worn = false },
        new ClothingItem { type = ClothingType.Shirt, worn = false },
        new ClothingItem { type = ClothingType.Pants, worn = false }
    };

    public bool IsNude => CurrentOutfit.Count == 0;

    public List<ClothingItem> CurrentOutfit => clothingItems.FindAll(item => item.worn);

    /// <summary>
    /// Puts on a piece of clothing
    /// </summary>
    /// <param name="type">The type of clothing to put on</param>
    public void PutOnClothing(ClothingType type)
    {
        var item = clothingItems.Find(i => i.type == type);
        if (item != null)
        {
            item.worn = true;
        }
    }

    /// <summary>
    /// Removes a random piece of clothing
    /// </summary>
    private void RemoveRandomClothing()
    {
        if (IsNude) { return; }

        var wornItems = CurrentOutfit;
        int randomItem = Random.Range(0, wornItems.Count);
        wornItems[randomItem].worn = false;
    }

    /// <summary>
    /// Damages the player by removing a piece of clothing,
    /// or ending the game if the player is already nude
    /// </summary>
    [ContextMenu("Damage Player")]
    public void Damage()
    {
        if (IsNude)
        {
            GameManager.instance.gameoverEvent.Invoke();
        }
        else
        {
            RemoveRandomClothing();
        }
    }

    public override string ToString()
    {
        string status = "Current Outfit:\n";
        status += "Amount of clothing items: " + CurrentOutfit.Count + "\n";
        foreach (var item in clothingItems)
        {
            status += $"{item.type}: {(item.worn ? "Worn" : "Not Worn")}\n";
        }
        return status;
    }

    [ContextMenu("Log Status")]
    private void LogStatus() => Debug.Log(this);
}