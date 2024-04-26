using System.Collections.Generic;

[System.Serializable]
public struct TileProbability
{
    public TileData tileData;
    public float probability;
}

[System.Serializable]
public struct RowSetting
{
    public TileData rowTileData;    // tile for this row, ie water or road
    public int amountOfRows;  // amount of rows to occur on each grid chunk
}


[System.Serializable]
public struct DifficultyProfile
{
    public List<TileProbability> tileProbabilities;
    public List<RowSetting> rowSettings;
    public int amtOfWaterRows;
}