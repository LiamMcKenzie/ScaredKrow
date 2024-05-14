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

    #region Singleton
    public static GameManager instance;
    public int score = 0;
    private int backMovements = 0;
    private PlayerMovement player;
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
        player = FindObjectOfType<PlayerMovement>();
    }

    /// <summary>
    /// calculate the score based on the players movement.
    /// The function is called in the playermovement script.
    /// </summary>
    /// <param name="movement"></param>
    public void CalculateScore(Vector3 movement)
    {
        if (movement == Vector3.right) //checks if player is moving forward. (for some reason vector.right is forward)
        {
            if(backMovements > 0) //backmovements checks the amount of times the player has moved back without moving forward.
            {
                backMovements--;
            }else{ 
                score++; //increases the score by 1
            }
        }
        else if (movement == Vector3.left)
        {
            backMovements++;
        }
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