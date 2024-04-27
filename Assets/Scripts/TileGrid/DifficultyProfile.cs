/*
 * File: DifficultyProfile.cs
 * Purpose: Defines a class and helper structs used for tile generation
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
/// A class containing the tile probabilities and the amounts of the various rows
/// This is used by TileGridChunk.GenerateTileGrid to generate a chunk of tiles
/// </summary>
[System.Serializable]
public class DifficultyProfile
{
    public List<TileProbability> tileProbabilities;
    public List<RowSetting> rowSettings;

    public void ShuffleProbabilities()
    {
        Shuffle(tileProbabilities);
        Shuffle(rowSettings);
    }

    /// <summary>
    /// Shuffle a list with the Fisher-Yates algorithm
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <param name="list">The list to shuffle</param>
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}