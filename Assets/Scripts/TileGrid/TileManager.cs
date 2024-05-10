/*
 * File: TileManager.cs
 * Purpose: Manage the grid of tiles
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

/// <summary>
/// Manages the grid of tiles
/// The grid is composed of two grid chunks
/// </summary>
public class TileManager : MonoBehaviour
{
    public UnityEvent GridRepositioned;
    public GameObject playerPrefab;
    [Header("Tile Settings")]
    //public Vector2 playerStartCoords = new(1, 7);    // The starting coordinates of the player  NOTE: is this unused? there is also a playerstartcoords value in gamemanager
    public TileData defaultTileData;    // The default tile data (empty grass tile)
    public Color defaultTileColor = new(200, 183, 65);  // The base color of the tiles, defaults to a hayish color
    public float altRowDarkAmt = 0.9f;  // The amount to darken the alternate rows by

    [Header("Tile Generation Settings")]
    public DifficultyProfile difficultyProfile; // The difficulty profile, a struct containing the tile probabilities and the amount of water rows

    [Header("Grid Settings")]
    [SerializeField] private TileGridChunk[] gridChunks = new TileGridChunk[2]; // The two grid chunks, should be game objects with a TileGridChunk component
    [SerializeField] private int tilesWide = 15;    // width of each grid chunk (z-axis)
    [SerializeField] public int tilesHigh = 10;   // height of each grid chunk (x-axis)
    [SerializeField] private float tileSize = 1f;   // size of each tile

    [Header("Speed Settings")]
    [SerializeField] private float _speed = 1; // backing field for Speed property
    [SerializeField] private float maxSpeed = 500; // maximum speed

    [Header("Optimisation Settings")]
    [SerializeField] private bool deactivateTilesOutsideViewport = true; // Whether to optimise the tiles (turns them off when they are not visible)
    [SerializeField] private BufferArea viewportBufferArea = new(0.2f, 0.2f, 0.2f, 0.2f); // The buffer area around the camera viewport
    [SerializeField] private GameManager gameManager; // The game manager

    private Camera mainCamera;  // The main camera
    private GameObject player;  //player game object

    //Changed to be public for testing, was originally private -Liam
    public List<List<TileController>> masterTileControllerList = new();    // The master tile controller list, a 2D list of all the tile controllers in the grid

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
    /// The speed at which the tiles move
    /// </summary>
    /// <value>Speed should be greater than or equal to 0</value>
    public float Speed
    {
        get { return _speed; }
        set 
        {
            // Ensure _speed is proper range
            _speed = Mathf.Clamp(value, 0, maxSpeed);
        }
    }

    /// <summary>
    /// The starting position of the upper chunk
    /// </summary>
    private Vector3 UpperChunkPosition => new(tilesHigh * tileSize, 0, 0);

    /// <summary>
    /// This is called when the script is loaded or a value is changed in the inspector
    /// Its a workaround for Unity not serializing properties, so the setter logic can be applied when the value is changed in the inspector
    /// </summary>
    void OnValidate()
    {
        Speed = _speed;
    }

    /// <summary>
    /// On Start, generate the tile chunks and populate the master grid list
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Every frame, move the grid chunks and check if they need to be repositioned
    /// </summary>
    void Update()
    {
        if (gameManager.gameStarted == false) return;
        foreach (TileGridChunk gridChunk in gridChunks)
        {
            gridChunk.MoveTiles(Speed);
        }

        CheckAndRepositionGridChunks();

        if (deactivateTilesOutsideViewport)
        {
            HandleViewport();
        }
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void InitTileGrid()
    {
        
        gridChunks[0].transform.position = Vector3.zero;
        gridChunks[1].transform.position = UpperChunkPosition;

        foreach (TileGridChunk gridChunk in gridChunks)
        {
            gridChunk.GenerateTileGrid(tilesWide, tilesHigh, tileSize, difficultyProfile);
        }

        PopulateMasterGrid(gridChunks[0]);
    }

    /// <summary>
    /// Check if the grid chunks have moved off the screen and if so reposition them
    /// </summary>
    private void CheckAndRepositionGridChunks()
    {
        foreach (TileGridChunk gridChunk in gridChunks)
        {
            if (GridChunkHasMovedOffScreen(gridChunk))
            {
                RepositionGridChunk(gridChunk);
                break;
            }
        }
    }

    /// <summary>
    /// Check if the grid chunk has moved off the screen
    /// </summary>
    /// <param name="tileGrid">The grid chunk to check</param>
    /// <returns>Whether the grid chunk has moved off the screen</returns>
    private bool GridChunkHasMovedOffScreen(TileGridChunk tileGrid) => tileGrid.transform.position.x < -tilesHigh * tileSize;

    /// <summary>
    /// Checks if the grid chunk has moved off the screen and if so generates a new grid above the screen
    /// </summary>
    /// <param name="tileGrid">The grid chunk to check</param>
    /// <param name="otherTileGrid">The other grid chunk</param>
    private void RepositionGridChunk(TileGridChunk gridChunk)
    {
        TileGridChunk otherGridChunk = GetOtherGridChunk(gridChunk);

        gridChunk.transform.position = UpperChunkPosition;
        gridChunk.GenerateTileGrid(tilesWide, tilesHigh, tileSize, difficultyProfile);

        // Position the other tile group at 0 (this ensures there is no gap between the two chunks)
        otherGridChunk.RecenterTiles();
        
        PopulateMasterGrid(otherGridChunk);

        GridRepositioned.Invoke();
    }

    /// <summary>
    /// Get the other grid chunk
    /// </summary>
    /// <param name="gridChunk">The grid chunk to check</param>
    /// <returns>The other grid chunk</returns>
    private TileGridChunk GetOtherGridChunk(TileGridChunk gridChunk) => (gridChunk == gridChunks[0]) ? gridChunks[1] : gridChunks[0];

    /// <summary>
    /// Populate the master grid list with the tile controllers from the two grid chunks
    /// </summary>
    /// <param name="bottomGridChunk">The bottom grid chunk</param>
    private void PopulateMasterGrid(TileGridChunk bottomGridChunk)
    {
        masterTileControllerList.Clear();

        TileGridChunk topGridChunk = GetOtherGridChunk(bottomGridChunk);

        masterTileControllerList.AddRange(bottomGridChunk.tileControllerList);
        masterTileControllerList.AddRange(topGridChunk.tileControllerList);

        ResetTileCoordinates();
    }

    /// <summary>
    /// Resets the tile coordinates after repositioning the grid chunks
    /// The coordinates of each tile will then be the same as their position in the masterTileControllerList
    /// </summary>
    private void ResetTileCoordinates()
    {
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

    /// <summary>
    /// Handle the viewport
    /// If the tiles are outside the viewport, deactivate them to improve performance
    /// NOTE: If the directional light is at a dramatic angle, shadows may pop in and out
    /// To account for this, the buffer area should be increased in the direction of the light
    /// </summary>
    /// <remarks>
    /// This method could also be used to limit player movement to the viewport
    /// By iterating over the tiles with a negative buffer area, and setting those tiles to impassable, the player could be limited to a certain area within the viewport
    /// </remarks>
    private void HandleViewport()
    {
        //Player camera movement
        
        Vector3 playerPosInViewport = mainCamera.WorldToViewportPoint(player.transform.position);
        Debug.Log(playerPosInViewport);

        float speedPoint = 0.3f; //when should the camera speed up, 0.5 half way up the screen
        float speedIncrease = 5f; //used in the lerp function

        if (playerPosInViewport.y > speedPoint)
        {
            //normalize position from range 0.5-1 to 0-1
            float normalizedPosition = (playerPosInViewport.y - speedPoint) / speedPoint;
            float exponentialPosition = Mathf.Pow(normalizedPosition, 2); 
            Speed = Mathf.Lerp(0.5f, speedIncrease, exponentialPosition);
        }
        else
        {
            Speed = 0.5f;
        }

        foreach (var row in masterTileControllerList)
        {
            foreach (var tile in row)
            {
                bool isTileInViewport = tile.IsInViewport(mainCamera, viewportBufferArea);
                // check if the active state matches the bool returned by IsInViewport
                // this is to avoid unnecessary calls to SetActive which can be expensive
                if (tile.gameObject.activeSelf != isTileInViewport)
                {
                    tile.gameObject.SetActive(isTileInViewport);
                }
            }
        }
    }

    /// <summary>
    /// Select this in the inspector to print the grid to the console
    /// </summary>
    [ContextMenu("Print Grid")]
    public void PrintGrid() => Debug.Log(ToString());

    /// <summary>
    /// Iterate through each tile and return a string representation of the grid
    /// </summary>
    /// <returns>A string representation of the grid</returns>
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

    /// <summary>
    /// Instantiate a prefab on a tile
    /// </summary>
    /// <param name="prefab">The prefab to instantiate</param>
    /// <param name="coords">The coordinates of the tile to instantiate the prefab on</param>
    public void InstantiateOnTile(GameObject prefab, TileGridCoords coords)
    {
        masterTileControllerList[coords.x][coords.z].InstantiateOnThisTile(prefab);
    }
}