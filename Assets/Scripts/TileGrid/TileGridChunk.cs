using System.Collections.Generic;
using UnityEngine;

public class TileGridChunk : MonoBehaviour
{
    public List<List<TileController>> tileControllerList = new(); // 2D list of tile controllers in this chunk
    private TileManager tileManager;

    void Start()
    {
        tileManager = TileManager.instance;
    }

    public void MoveTiles(float speed) => transform.position += Vector3.left * speed * Time.deltaTime;

    public void RecenterTiles() => transform.position = Vector3.zero;

    public void GenerateTileGrid(Vector3 position, int tilesWide, int tilesHigh, float tileSize, DifficultyProfile difficultyProfile)
    {
        ClearTiles();

        Dictionary<int, TileData> rowDictionary = GenerateRows(tilesHigh, difficultyProfile.rowSettings);

        for (int x = 0; x < tilesHigh; x++)
        {
            List<TileController> row = new();

            for (int z = 0; z < tilesWide; z++)
            {
                TileData tileData = SelectTileData(x, rowDictionary, difficultyProfile.tileProbabilities);

                TileController tileController = tileData.CreateTileController(transform, x, z);

                Vector3 worldPosition = CalculateWorldPosition(x, z, tileSize, position);
                tileController.InstantiateTile(worldPosition, this, x % 2 == 0);

                row.Add(tileController);
            }
            tileControllerList.Add(row);
        }
    }

    private Dictionary<int, TileData> GenerateRows(int tilesHigh, List<RowSetting> rowSettings)
    {
        Dictionary<int, TileData> rowDictionary = new();

        foreach (RowSetting rowSetting in rowSettings)
        {
            for (int i = 0; i < rowSetting.amountOfRows; i++)
            {
                int randomRow = Random.Range(0, tilesHigh);
                if (rowDictionary.ContainsKey(randomRow) == false)
                {
                    rowDictionary.Add(randomRow, rowSetting.rowTileData);
                }
            }
        }

        return rowDictionary;
    }

    private TileData SelectTileData(int x, Dictionary<int, TileData> rowDictionary,List<TileProbability> tileProbabilities)
    {
        if (rowDictionary.ContainsKey(x))
        {
            return rowDictionary[x];
        }

        float totalProbability = 100;
        float randomProbability = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        foreach (TileProbability tileProbability in tileProbabilities)
        {
            cumulativeProbability += tileProbability.probability;
            if (randomProbability <= cumulativeProbability)
            {
                return tileProbability.tileData;
            }
        }

        return tileManager.defaultTileData;
    }

    private Vector3 CalculateWorldPosition(int x, int z, float tileSize, Vector3 position) => new Vector3(x * tileSize, 0, z * tileSize) + position;

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