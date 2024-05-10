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
    #region Singleton
    public static GameManager instance;
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
    [Header("Game Settings")]
    public bool gameStarted; // Whether the game has started
    [SerializeField] private Camera mainCamera; // The main camera
    [SerializeField] private GameoverTriggerArea gameoverTriggerArea; // The game over trigger area
    [SerializeField] private GameObject GameoverPanel; // The game over panel

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab; // The player prefab
    public TileGridCoords playerStartCoords = new(x: 5, z: 5); // The starting coordinates of the player
    public PlayerController playerController; // The player controller

    [Header("Map Settings")]
    [SerializeField] private TileManager tileManager; // The tile manager

    [Header("Speed Settings")]
    [SerializeField] private float initSpeed = 1; // The initial speed
    [SerializeField] private float speedIncrement = 0.05f; // backing field for Speed property
    [SerializeField] private float _speed = 1; // backing field for Speed property
    [SerializeField] private float maxSpeed = 500; // maximum speed
    [SerializeField] private float catchupSpeed = 2; // The speed at which the player catches up
    [SerializeField] private float accelerationCurve = 1.5f; // The speed at which the player catches up

    private float speedBeforeCatchup; // The speed before catchup is initiated
    private bool isCatchingUp;
    public bool playerInCatchupZone = false;


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
    /// This is called when the script is loaded or a value is changed in the inspector
    /// Its a workaround for Unity not serializing properties, so the setter logic can be applied when the value is changed in the inspector
    /// </summary>
    void OnValidate()
    {
        Speed = _speed;
    }

    void Start()
    {
        speedBeforeCatchup = Speed;
        gameoverTriggerArea.gameoverEvent.AddListener(GameOver);
        Init();
    }

    void Update()
    {
        if (gameStarted == false) return;

        // Check if the player is in the catchup zone
        if (playerInCatchupZone)
        {
            Catchup();
        }
        else
        {
            StopCatchup();
        }

        Speed += speedIncrement * Time.deltaTime;


    }
    /// <summary>
    /// Initialise game state
    /// </summary>
    public void Init()
    {
        GameoverPanel.SetActive(false);
        tileManager.InitTileGrid();
        SpawnPlayer();
        gameStarted = true;
    }

    /// <summary>
    /// Reset the game
    /// </summary>
    public void Reset()
    {
        Speed = initSpeed;
        Init();
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// End the game
    /// </summary>
    public void GameOver()
    {
        gameStarted = false;
        GameoverPanel.SetActive(true);
        Destroy(playerController.gameObject);
    }

    /// <summary>
    /// Spawn the player at the starting coordinates
    /// </summary>
    private void SpawnPlayer()
    {
        playerController = tileManager.InstantiateOnTile(playerPrefab, playerStartCoords).GetComponent<PlayerController>();
    }

    /// <summary>
    /// Increase game speed to catch up to the player
    /// </summary>
    private void Catchup()
    {
        // If we are not already catching up, store the speed before catchup
        if (!isCatchingUp) { speedBeforeCatchup = Speed; }

        // accelerate to catchup speed
        Speed = Mathf.Lerp(Speed, catchupSpeed, Time.deltaTime * accelerationCurve);

        isCatchingUp = true;
    }

    /// <summary>
    /// Stop catching up to the player
    /// </summary>
    private void StopCatchup()
    {
        if (isCatchingUp == false) return;

        // decelerate from catchup speed to the speed before catchup
        Speed = Mathf.Lerp(Speed, speedBeforeCatchup, Time.deltaTime * accelerationCurve);

        // if we are close enough to the speed before catchup, stop catching up
        if (Mathf.Abs(Speed - speedBeforeCatchup) < 0.1f)
        {
            Speed = speedBeforeCatchup;
            isCatchingUp = false;
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

    // override ToString to return a nicely formatted string
    public override string ToString()
    {
        return $"({x}, {z})";
    }
}