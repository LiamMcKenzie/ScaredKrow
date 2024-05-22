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
    private const int STARTING_HP = 1;
    [field: SerializeField] public int CurrentHP { get; private set;}

    private List<ClothingItem> clothingItems = new List<ClothingItem>() {
        new ClothingItem { type = ClothingType.Hat, worn = true },
        new ClothingItem { type = ClothingType.Shirt, worn = true },
        new ClothingItem { type = ClothingType.Pants, worn = true }
    };

    public bool IsNude => CurrentHP <= STARTING_HP;

    public List<ClothingItem> CurrentOutfit => clothingItems.FindAll(item => item.worn);

    private void Start()
    {
        CalculateHealth();
    }

    public void MakeNude()
    {
        foreach (var item in clothingItems)
        {
            item.worn = false;
        }
        CurrentHP = STARTING_HP;
    }

    private void CalculateHealth()
    {
        CurrentHP = STARTING_HP + CurrentOutfit.Count;
    }

    private void RemoveRandomClothing()
    {
        if (IsNude) { return; }

        var wornItems = CurrentOutfit;
        int randomItem = Random.Range(0, wornItems.Count);
        wornItems[randomItem].worn = false;

        CalculateHealth();
    }

    [ContextMenu("Damage Player")]
    public void Damage()
    {
        RemoveRandomClothing();
        if (IsNude)
        {
            CurrentHP = 0;
            GameManager.instance.gameoverEvent.Invoke();
        }
    }

    public override string ToString()
    {
        string status = $"Player Health: {CurrentHP}\n";
        status += "Current Outfit:\n";
        foreach (var item in clothingItems)
        {
            status += $"{item.type}: {(item.worn ? "Worn" : "Not Worn")}\n";
        }
        return status;
    }

    [ContextMenu("Log Status")]
    private void LogStatus() => Debug.Log(this);
}