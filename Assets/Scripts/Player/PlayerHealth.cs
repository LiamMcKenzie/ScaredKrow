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

public class PlayerHealth : MonoBehaviour
{
    private List<ClothingItem> clothingItems = new List<ClothingItem>() {
        new ClothingItem { type = ClothingType.Hat, worn = false },
        new ClothingItem { type = ClothingType.Shirt, worn = false },
        new ClothingItem { type = ClothingType.Pants, worn = false }
    };

    public bool IsNude => CurrentOutfit.Count == 0;

    public List<ClothingItem> CurrentOutfit => clothingItems.FindAll(item => item.worn);

    public void MakeNude()
    {
        foreach (var item in clothingItems)
        {
            item.worn = false;
        }
    }

    public void PutOnClothing(ClothingType type)
    {
        var item = clothingItems.Find(i => i.type == type);
        if (item != null)
        {
            item.worn = true;
        }
    }

    private void RemoveRandomClothing()
    {
        if (IsNude) { return; }

        var wornItems = CurrentOutfit;
        int randomItem = Random.Range(0, wornItems.Count);
        wornItems[randomItem].worn = false;
    }

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