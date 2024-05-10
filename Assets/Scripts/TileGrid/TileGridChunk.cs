/*
 * File: TileGridChunk.cs
 * Purpose: Generate and manage a chunk of tiles
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used by TileManager to generate the grid of tiles
/// Two of these chunks are used to create the grid
/// This is where the procedural generation of the tiles occurs
/// </summary>
public class TileGridChunk : MonoBehaviour
{
    public List<List<TileController>> tileControllerList = new(); // 2D list of tile controllers in this chunk
    public TileManager tileManager;
    public GameManager gameManager;

    void Start()
    {
        // log if gameManger is null
        if (gameManager == null) { Debug.Log("GameManager is null"); }
        tileManager = TileManager.instance;
    }

    /// <summary>
    /// Move the tiles down the x-axis
    /// </summary>
    /// <param name="speed">The speed at which to move the tiles</param>
    public void MoveTiles(float speed) => transform.position += Vector3.left * speed * Time.deltaTime;

    /// <summary>
    /// Recenter the tiles
    /// </summary>
    public void RecenterTiles() => transform.position = Vector3.zero;

    /// <summary>
    /// Generate a chunk of the tile grid
    /// </summary>
    /// <param name="position">The position of the grid chunk</param>
    /// <param name="tilesWide">The number of tiles wide (x-axis)</param>
    /// <param name="tilesHigh">The number of tiles high (z-axis)</param>
    /// <param name="tileSize">The size of the tiles</param>
    /// <param name="difficultyProfile">Contains the probabilities/amounts of the various tile types</param>
    public void GenerateTileGrid(int tilesWide, int tilesHigh, float tileSize, DifficultyProfile difficultyProfile)
    {
        int boundaryLeft = difficultyProfile.boundaryLeft;
        int boundaryRight = difficultyProfile.boundaryRight;

        // Clear the existing chunk
        ClearTiles();

        // This dictionary will store the row number (x) and the tile data for that row
        Dictionary<int, TileData> rowDictionary = GenerateRows(tilesHigh, difficultyProfile.rowSettings);

        // Begin iterating through the rows
        for (int x = 0; x < tilesHigh; x++)
        {
            // This list will store the tile controllers for each row
            List<TileController> row = new();

            // Begin iterating through the cells in the row
            for (int z = 0; z < tilesWide; z++)
            {
                // Get the tile data for this cell
                TileData tileData = SelectTileData(x, z, rowDictionary, difficultyProfile.tileProbabilities, difficultyProfile.boundaryLeft, difficultyProfile.boundaryRight);

                // Create a tile controller for this cell
                TileController tileController = tileData.CreateTileController(transform, x, z);


                // Calculate the world position of the cell
                Vector3 worldPosition = CalculateWorldPosition(x, z, tileSize);

                // bool isLight = x % 2 == 0;

                bool withinBounds = z >= boundaryRight && z <= tilesWide - boundaryLeft;
                bool evenRow = x % 2 == 0;

                // Instantiate the tile prefab at the world position
                tileController.InstantiateTile(worldPosition, this, light: withinBounds && evenRow);
                
                // Check if it should be a fence tile
                bool isRight = z < boundaryRight;
                if (z == boundaryRight - 1 || z == tilesWide - boundaryLeft + 1)
                {
                    tileController.BuildFence(isRight);
                }

                // Add the tile controller to the row list
                row.Add(tileController);
            }
            // Add the row to the outer layer of the 2D list
            tileControllerList.Add(row);
        }
    }

    /// <summary>
    /// Generate a dictionary of random row indices and the tile data for that row
    /// </summary>
    /// <param name="tilesHigh">The number of tiles high (x-axis)</param>
    /// <param name="rowSettings">A list of row settings. These are structs that containing the tile data for the row and the amount of times the row should be repeated on this chunk</param>
    private Dictionary<int, TileData> GenerateRows(int tilesHigh, List<RowSetting> rowSettings)
    {
        // This dictionary will store the row number (x) and the tile data for that row
        Dictionary<int, TileData> rowDictionary = new();

        
        // Iterate through each row setting
        foreach (RowSetting rowSetting in rowSettings)
        {
            // Iterate through the amount of times the row should be repeated
            for (int i = 0; i < rowSetting.amountOfRows; i++)
            {
                // Generate a random row number within the bounds of the grid
                int randomRow = Random.Range(0, tilesHigh);

                // If the row number is not already in the dictionary, add it
                // This could be a while loop to ensure each row is added, but the distribution seems pretty good as is
                if (rowDictionary.ContainsKey(randomRow) == false )
                {
                    rowDictionary.Add(randomRow, rowSetting.rowTileData);
                }
            }
        }

        return rowDictionary;
    }

    /// <summary>
    /// Select the tile data for a given cell
    /// </summary>
    /// <param name="x">This is the loop counter from the outer for loop in GenerateTileGrid. This corresponds to the row number of the grid chunk.</param>
    /// <param name="rowDictionary">A dictionary containing random row number indices (x) and the tile data for that row</param>
    /// <param name="tileProbabilities">A list of tile probabilities. These are structs that contain the tile data and the probability of that tile appearing</param>
    private TileData SelectTileData(int x, int z, Dictionary<int, TileData> rowDictionary, List<TileProbability> tileProbabilities, int boundaryLeft, int boundaryRight)
    {
        // make the player start row a default tile
        if (gameManager.gameStarted == false && x == gameManager.playerStartCoords.x ) 
        { 
            return tileManager.defaultTileData;
        }

        // If the row number is in the dictionary, return the tile data for that row
        if (rowDictionary.ContainsKey(x))
        {
            return rowDictionary[x];
        }

        // if z is less than the left boundary or greater than the right boundary, return the default tile
        if (z < boundaryRight || z > tileManager.tilesWide - boundaryLeft) 
        {
            return tileManager.defaultTileData;
        }

        // Otherwise, select a tile data based on the probabilities    

        // There's a total probability of 100% for all tiles
        float totalProbability = 100;

        // Generate a random probability
        float randomProbability = Random.Range(0, totalProbability);

        // This will be used to calculate the cumulative probability
        float cumulativeProbability = 0;    

        // Iterate through the tile probabilities
        foreach (TileProbability tileProbability in tileProbabilities)
        {
            // Add the probability of the current tile to the cumulative probability
            cumulativeProbability += tileProbability.probability;
            // If the random probability is less than or equal to the cumulative probability, return that tile data
            if (randomProbability <= cumulativeProbability)
            {
                return tileProbability.tileData;
            }
        }

        // If no tile data is selected, return the default tile data
        return tileManager.defaultTileData;
    }

    /// <summary>
    /// Calculate the world position of a cell
    /// Multiplies the x and z coordinates by the tile size and add the positions of the grid chunk
    /// </summary>
    /// <param name="x">The x coordinate of the cell</param>
    /// <param name="z">The z coordinate of the cell</param>
    /// <param name="tileSize">The size of the tile</param>
    /// <param name="position">The position of the grid chunk</param>
    /// <returns>The world position of the cell</returns>
    private Vector3 CalculateWorldPosition(int x, int z, float tileSize) => new Vector3(x * tileSize, 0, z * tileSize) + transform.position;

    /// <summary>
    /// Clear the existing chunk of tiles
    /// </summary>
    private void ClearTiles()
    {
        if (tileControllerList.Count == 0) { return; }
        
        foreach (var row in tileControllerList)
        {
            foreach (var tileController in row)
            {
                Destroy(tileController.gameObject);
            }
        }

        tileControllerList.Clear();
    }
}