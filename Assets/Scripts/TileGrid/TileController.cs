/*
 * File: TileController.cs
 * Purpose: Controls a tile and its state
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
/// This class is used to control the tile
/// It is used to instantiate the tile prefab and manage the tile over its lifetime
/// </summary>
public class TileController : MonoBehaviour
{
    [Header("Tile Coordinates")]
    [SerializeField] public int x; // The x coordinate of the tile
    [SerializeField] public int z; // The z coordinate of the tile

    private const float Z_ADJUST_AMT = 0.45f; // The amount to adjust the z position of the fence by
    private const float X_ADJUST_AMT = 0.5f; // The amount to adjust the x position of the fence by
    private const float CROSSING_Y_OFFSET = 0.02f; // The amount to offset the crossing by

    private GameObject tilePrefab;  // The prefab of the tile
    private GameObject fencePrefab; // The fence mesh for this tile
    private GameObject crossingPrefab;  // The crossing mesh for this tile
    public bool isPassable;    // Can the player walk on this tile //NOTE: changed from private to public. For use in PlayerMovement check -Liam
    private bool isHidingPlace; // Can the player hide in this tile
    private bool isRotatable;   // Does the tile contain a rotatable mesh (this should be the first child of the tile prefab if so) 
    public bool hasCrossings;  // Is this tile a fence
    public bool isACrossing;
    public bool containsPlayer;

    private const int NAME_LENGTH = 7;  // The length of the name to display in the ToString method
    private TileGridChunk parentChunk;  // The grid chunk this tile is in
    private Color color;    // The color of the tile (the quad that represents the tile)
    private GameObject tileInstance;    // The instance of the tile prefab


    /// <summary>
    /// Initializes the tile controller with the tile data
    /// </summary>
    /// <param name="tileData">The tile data scriptable object</param>
    /// <param name="x">The x coordinate of the tile</param>
    /// <param name="z">The z coordinate of the tile</param>
    public void InitializeTileController(TileData tileData, int x, int z)
    {
        this.x = x;
        this.z = z;
        tilePrefab = tileData.tilePrefab;
        isPassable = tileData.isPassable;
        isHidingPlace = tileData.isHidingPlace;
        isRotatable = tileData.isRotatable;
        hasCrossings = tileData.hasCrossings;
        fencePrefab = tileData.fencePrefab;
        crossingPrefab = tileData.crossingPrefab;
    }

    /// <summary>
    /// Instantiates the tile prefab at the given position
    /// </summary>
    /// <param name="position">The position to instantiate the tile at</param>
    /// <param name="parentChunk">The parent chunk of the tile</param>
    /// <param name="light">Whether the tile should be light or dark</param>
    /// <returns>The instance of the tile prefab</returns>
    public GameObject InstantiateTile(Vector3 position, TileGridChunk parentChunk, bool light = true)
    {
        color = TileManager.instance.defaultTileColor;
        this.parentChunk = parentChunk;
        transform.position = position;
        tileInstance = Instantiate(tilePrefab, position, tilePrefab.transform.rotation, transform);
        // If its rotatable, rotate the first child of the tile prefab
        if (isRotatable)
        {
            tileInstance.transform.GetChild(0)?.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        }
        // Set to light or dark color
        SetColor(light ? color : color * TileManager.instance.altRowDarkAmt);

        return tileInstance;
    }

    /// <summary>
    /// Builds a fence on this tile
    /// </summary>
    /// <param name="isRight">Whether the fence should be on the right side of the tile</param>
    /// <returns>The instance of the fence prefab</returns>
    public GameObject BuildFence(bool isRight)
    {
        isPassable = false;
        if (fencePrefab == null)
        {
            Debug.LogWarning("No fence prefab set for this tile");
            return null;
        }
        float zAdjust = isRight ? Z_ADJUST_AMT : -Z_ADJUST_AMT;
        float xAdjust = isRight ? X_ADJUST_AMT : -X_ADJUST_AMT;
        Vector3 relativePosition = new Vector3(xAdjust, 0, zAdjust);
        Vector3 worldPosition = transform.TransformPoint(relativePosition);
        Quaternion rotation = isRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        GameObject fenceInstance = Instantiate(fencePrefab, worldPosition, rotation, transform);
        return fenceInstance;
    }

    /// <summary>
    /// Instantiates a prefab on this tile and makes it a child of the tile
    /// </summary>
    /// <param name="prefab">The prefab to instantiate</param>
    public GameObject InstantiateOnThisTile(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform.position, prefab.transform.rotation, transform);
        // check if newobject has player tag
        if (newObject.CompareTag("Player"))
        {
            containsPlayer = true;
        }
        return newObject;
    }

    /// <summary>
    /// Sets the x and z coordinates of the tile
    /// </summary>
    /// <param name="x">The x coordinate of the tile</param>
    /// <param name="z">The z coordinate of the tile</param>
    public void SetCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    /// <summary>
    /// Override ToString and return a nicely formatted string
    /// </summary>
    /// <returns>A string with the parent grid chunk, tile name, and coordinates</returns>
    public override string ToString()
    {
        string result = "";
        string chunkLetter = parentChunk.gameObject.name[^1..];
        string tileName = (gameObject.name.Length > NAME_LENGTH) ? $"{gameObject.name[..(NAME_LENGTH - "...".Length)]}..." : gameObject.name;
        result += $"({x},{z}) {chunkLetter} {tileName}\t";
        return result;
    }

    /// <summary>
    /// Checks if the tile is in the camera viewport
    /// </summary>
    /// <param name="camera">The camera to check the viewport of</param>
    /// <param name="bufferArea">The buffer area around the camera viewport (see below for the struct definition)</param>
    /// <returns>Whether the tile is in the camera viewport</returns>
    /// <remarks>
    /// This will also be useful for keeping the player in the play area
    /// And also for checking path viability
    /// </remarks>
    public bool IsInViewport(Camera camera, BufferArea bufferArea = new())
    {
        // Convert the world position of the tile to a viewport position
        // The viewport position is a Vector3 with values between 0 and 1
        Vector3 screenPoint = camera.WorldToViewportPoint(transform.position);
        bool inViewport = screenPoint.x > 0 - bufferArea.left && screenPoint.x < 1 + bufferArea.right && screenPoint.y > 0 - bufferArea.down && screenPoint.y < 1 + bufferArea.up;
        return inViewport;
    }

    /// <summary>
    /// Sets the color of the tile (the quad that represents the tile)
    /// </summary>
    public void SetColor(Color color)
    {
        this.color = color;
        tileInstance.GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// Makes the provided game object a child of the tile
    /// </summary>
    /// <param name="gameObject">The game object to make a child of the tile</param>
    public void PlayerEntersTile(Transform playerTransform)
    {
        containsPlayer = true;
        playerTransform.SetParent(transform);
    }

    /// <summary>
    /// Adds the crossing asset to the tile and sets the tile to passable
    /// </summary>
    public void AddCrossing()
    {
        if (crossingPrefab == null) { return; }
        isACrossing = true;
        isPassable = true;
        GameObject crossing = InstantiateOnThisTile(crossingPrefab);
        // crossing rotation y random
        crossing.transform.Rotate(new Vector3(0, Random.Range(-30, 30), 0));

        crossing.transform.localPosition = new Vector3(0, CROSSING_Y_OFFSET, 0);
    }

    /// <summary>
    /// Change this to a passable tile (used on tiles adjacent a crossing)
    /// </summary>
    public void MakePassable()
    {
        if (isPassable) { return; }
        
        isPassable = true;
        // destroy first child of first child (this is the mesh for an obstacle tile)
        Destroy(transform.GetChild(0)?.GetChild(0)?.gameObject);
        // append MadePassable to the name
        gameObject.name += "-MadePassable";
    }
}

/// <summary>
/// A struct to hold the buffer area around the camera viewport
/// </summary>
[System.Serializable]
public struct BufferArea
{
    public float left;
    public float right;
    public float up;
    public float down;

    public BufferArea(float left = 0, float right = 0, float up = 0, float down = 0)
    {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
    }
}
