/*
 * File: TileManager.cs
 * Purpose: Manage the grid of tiles
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

/// <summary>
/// Manages the grid of tiles
/// The grid is composed of two grid chunks
/// </summary>
public class TileManager : MonoBehaviour
{
    public TileData defaultTileData;    // The default tile data (empty grass tile)
    public TileData waterTileData;  // The water tile data
    public DifficultyProfile difficultyProfile; // The difficulty profile, a struct containing the tile probabilities and the amount of water rows
    public Color defaultTileColor = new(200, 183, 65);  // The base color of the tiles, defaults to a hayish color
    public float altRowDarkAmt = 0.9f;  // The amount to darken the alternate rows by

    [SerializeField] private int tilesWide = 15;    // width of each grid chunk (z-axis)
    [SerializeField] private int tilesHigh = 10;   // height of each grid chunk (x-axis)
    [SerializeField] private float tileSize = 1f;   // size of each tile
    [SerializeField] private float _speed = 1; // backing field for Speed property
    [SerializeField] private float maxSpeed = 500; // maximum _speed
    [SerializeField] private TileGridChunk gridChunkA;  // The first grid chunk, should be a game object with a TileGridChunk component
    [SerializeField] private TileGridChunk gridChunkB;  // The second grid chunk, should be a game object with a TileGridChunk component
    [SerializeField] private bool optimiseTiles = true; // Whether to optimise the tiles (turns them off when they are not visible)
    [SerializeField] private int baseOptimisationFrequency = 30; // The base optimisation frequency for a _speed of 1 (this value is divided by the _speed to get the optimisation frequency)

    private int optimisationFrequency;  // The frequency in frames at which the optimisation call is made
    private int optimiseCounter = 0;    // frame counter for optimisation
    private List<List<TileController>> masterTileControllerList = new();    // The master tile controller list, a 2D list of all the tile controllers in the grid

    #region Singleton
    public static TileManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// The _speed at which the tiles move
    /// </summary>
    /// <value>Speed should be greater than or equal to 0</value>
    public float Speed
    {
        get { return _speed; }
        set
        {
            // Ensure _speed is proper range
            _speed = Mathf.Clamp(value, 0, maxSpeed);
            
            // Update the optimisation rate when the _speed changes
            SetOptimisationRate();
        }
    }

    /// <summary>
    /// This is called when the script is loaded or a value is changed in the inspector
    /// Its a workaround for Unity not serializing properties, so the setter logic is applied when the value is changed in the inspector
    /// </summary>
    void OnValidate()
    {
        Speed = _speed;
    }

    /// <summary>
    /// Generate the tile chunks and populate the master grid list
    /// </summary>
    private void Start()
    {
        gridChunkA.transform.position = Vector3.zero;
        gridChunkB.transform.position = new Vector3(tilesHigh * tileSize, 0, 0);
        gridChunkA.GenerateTileGrid(Vector3.zero, tilesWide, tilesHigh, tileSize, difficultyProfile);
        gridChunkB.GenerateTileGrid(new Vector3(tilesHigh * tileSize, 0, 0), tilesWide, tilesHigh, tileSize, difficultyProfile);
        PopulateMasterGrid();
        SetOptimisationRate();
    }

    void Update()
    {
        MoveTiles();
        OptimiseTiles();
    }

    /// <summary>
    /// Moves the tiles along x-axis
    /// When they reach the end of the screen, they are moved to the other side 
    /// </summary>
    private void MoveTiles()
    {
        gridChunkA.MoveTiles(_speed);
        gridChunkB.MoveTiles(_speed);
        UpdateTileGridPosition(gridChunkA, gridChunkB);
        UpdateTileGridPosition(gridChunkB, gridChunkA);
    }

    private void UpdateTileGridPosition(TileGridChunk tileGrid, TileGridChunk otherTileGrid)
    {
        if (tileGrid.transform.position.x > -tilesHigh * tileSize) { return; }
        Vector3 newPosition = new(tilesHigh * tileSize, 0, 0);
        tileGrid.transform.position = newPosition;
        tileGrid.GenerateTileGrid(newPosition, tilesWide, tilesHigh, tileSize, difficultyProfile);
        optimiseCounter = optimisationFrequency;
        // Position the other tile group at 0
        otherTileGrid.RecenterTiles();

        PopulateMasterGrid();
    }

    private void PopulateMasterGrid()
    {
        // if gridchunkA has a lower x value than gridchunkB, the tiles of gridchunkA
        // should be occupy the the 0 tilesWide indices of the master grid
        // if gridchunkB has a lower x value than gridchunkA, the tiles of gridchunkB
        // should be occupy the the 0 tilesWide indices of the master grid
        // the other grid should occupy the tilesWide-2 tilesWide indices of the master grid
        masterTileControllerList.Clear();

        if (gridChunkA.transform.position.x < gridChunkB.transform.position.x)
        {
            masterTileControllerList.AddRange(gridChunkA.tileControllerList);
            masterTileControllerList.AddRange(gridChunkB.tileControllerList);
        }
        else
        {
            masterTileControllerList.AddRange(gridChunkB.tileControllerList);
            masterTileControllerList.AddRange(gridChunkA.tileControllerList);
        }
        for (int x = 0; x < masterTileControllerList.Count; x++)
        {
            List<TileController> row = masterTileControllerList[x];
            for (int z = 0; z < row.Count; z++)
            {
                TileController tileController = row[z];
                tileController.SetCoords(x, z);
            }
        }
    }


    [ContextMenu("Print Grid")]
    public void PrintGrid() => Debug.Log(ToString());

    public override string ToString()
    {
        string result = "";
        foreach (var row in masterTileControllerList)
        {
            foreach (var tileController in row)
            {
                result += tileController.ToString();
            }
            result += "\n";
        }
        return result;
    }

    // as _speed increases, the frequency of optimisation increases
    private void SetOptimisationRate() => optimisationFrequency = (_speed <= 1) ? baseOptimisationFrequency : baseOptimisationFrequency / (int)_speed;

    private void OptimiseTiles()
    {
        optimiseCounter++;

        if (optimiseTiles == false || optimiseCounter < optimisationFrequency) { return; }

        foreach (var row in masterTileControllerList)
        {
            // Only check the first tile in the row
            var firstInRow = row[0];
            var posX = firstInRow.transform.position.x;
            // Check if the first in row is within the screen bounds
            bool shouldBeActive = posX > 0 && posX < tilesHigh * tileSize;
            // If the row is already in the correct state, do nothing
            // This is written like this to avoid unnecessary calls to gameObject.SetActive
            if (firstInRow.gameObject.activeSelf != shouldBeActive)
            {
                // Set all the tiles in the row to the correct state
                foreach (var tileController in row)
                {
                    tileController.gameObject.SetActive(shouldBeActive);
                }
            }
        }

        optimiseCounter = 0;
    }

}