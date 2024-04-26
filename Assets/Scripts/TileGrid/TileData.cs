/*
 * File: TileData.cs
 * Purpose: Contains the data for a tile
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
/// This contains the data for a tile
/// It is used in the DifficultyProfile to determine the probability of each tile type
/// The tile data is used to create a tile controller
/// Each tile controller has its own copy of the tile data
/// The tile controller is used to instantiate the tile prefab and manage the tile over its lifetime
/// </summary>
/// <remarks>
/// Its debatable whether this class is necessary, as the prefab could just have the tile controller script attached and configured appropriately 
/// But I prefer this as its a nicer way to edit the tile settings without digging through the prefabs
/// </remarks>
[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public GameObject tilePrefab;
    public bool isPassable = true;
    public bool isHidingPlace = false;
    public bool isRotatable = false;

    /// <summary>
    /// Creates a new tile controller
    /// Passes the tile data to the tile controller
    /// </summary>
    public TileController CreateTileController(Transform parent, int x, int z)
    {
        // Create a new tile controller game object
        // This is the parent object for the tile with the tile controller script attached
        // The controller itself is called to instantiate the tile prefab as a child of this object
        TileController tile = new GameObject(tilePrefab.name).AddComponent<TileController>();
        tile.transform.SetParent(parent);
        tile.InitializeTileController(this, x, z);
        return tile;
    }
}