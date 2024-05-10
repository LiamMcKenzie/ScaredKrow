/*
 * File: GameManager.cs
 * Purpose: Manage the game state
 */

using System;
using UnityEngine;

/// <summary>
/// Manages the game state
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // The player prefab
    [SerializeField] private TileManager tileManager; // The tile manager
    public TileGridCoords playerStartCoords = new( x:5, z:5 ); // The starting coordinates of the player
    public bool gameStarted; // Whether the game has started
    public int score; 

    #region Singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            // log this
            Debug.Log("GameManager instantiated");
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        StartGame();
    }

    /// <summary>
    /// Start the game by initializing the tile grid and spawning the player
    /// </summary>
    public void StartGame()
    {
        tileManager.InitTileGrid();
        SpawnPlayer();
        gameStarted = true;
    }

    /// <summary>
    /// Spawn the player at the starting coordinates
    /// </summary>
    private void SpawnPlayer()
    {
        tileManager.InstantiateOnTile(playerPrefab, playerStartCoords);
    }
}

/// <summary>
/// A struct to store the coordinates of a tile in our grid
/// </summary>
[Serializable]
public struct TileGridCoords
{
    public int x;
    public int z;

    public TileGridCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}