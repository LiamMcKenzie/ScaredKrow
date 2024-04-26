/*
 * File: TileData.cs
 * Purpose: Defines structs used for tile generation
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using System.Collections.Generic;

/// <summary>
/// A struct containing a tile data and its probability
/// This is for tiles that occur individually, like rocks and trees
/// </summary>
[System.Serializable]
public struct TileProbability
{
    public TileData tileData;   // tile data for this tile
    public float probability;   // probability of this tile (should be between 0 and 100)
}

/// <summary>
/// A struct containing a tile data intended to occur in a row and the amount of rows
/// </summary>
[System.Serializable]
public struct RowSetting
{
    public TileData rowTileData;    // tile for this row, ie water or road
    public int amountOfRows;  // amount of rows to occur on each grid chunk
}

/// <summary>
/// A struct containing the tile probabilities and the amounts of the various rows
/// This is used by TileGridChunk.GenerateTileGrid to generate a chunk of tiles
[System.Serializable]
public struct DifficultyProfile
{
    public List<TileProbability> tileProbabilities;
    public List<RowSetting> rowSettings;
}