/*
 * File: DifficultyProfile.cs
 * Purpose: Defines structs used for tile generation
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct containing a tile data and its probability
/// This is for tiles that occur individually, like rocks and trees
/// </summary>
[System.Serializable]
public struct TileProbability
{
    public TileData tileData;   // tile data for this tile
    public float probability;   // probability of this tile (should be between 0 and 100, anything above 100 is effectively 100% and anything below 0 is effectively 0%)
    public bool onlyOccursInsideBoundary; // Whether this tile can occur outside the boundary
    public bool onlyOccursOutsideBoundary; // Whether this tile can only occur outside the boundary
}

/// <summary>
/// A struct containing a tile data intended to occur in a row and the amount of rows
/// </summary>
[System.Serializable]
public struct RowSetting
{
    public TileData rowTileData;    // tile for this row, ie water or road
    public int amountOfRows;  // amount of rows to occur on each grid chunk
    public int minimumRowsApart;    // minimum amount of rows to occur between each row
}

/// <summary>
/// A class containing the tile probabilities and the amounts of the various rows
/// This is used by TileGridChunk.GenerateTileGrid to generate a chunk of tiles
/// </summary>
[System.Serializable]
public struct DifficultyProfile
{
    public List<TileProbability> tileProbabilities;
    public List<RowSetting> rowSettings;

    [Tooltip("Should be between 1 and 100, but there will always be at least one crossing")]
    public float crossingProbability;
    public int boundaryLeft, boundaryRight; // The index of the columns that define the boundary
}