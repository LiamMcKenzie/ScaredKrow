// scriptable object ClothingObject.cs:
// enum ClothingType, string style, gameobjact totemPrefab

// scriptable object ClothingObject.cs:

using UnityEngine;

[CreateAssetMenu]
public class ClothingData : ScriptableObject
{
    public ClothingType type;
    public string style;
    public GameObject totemPrefab;
}