/*
 * File: GameManager.cs
 * Purpose: Manage the game state
 */

using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the game state
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    public float PickupProbability = 5f;

    public int score = 0;
    private int backMovements = 0;
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
    public bool gameOver = false; // Whether the game has ended
    [SerializeField] private Camera mainCamera; // The main camera
    [SerializeField] private GameObject GameoverPanel; // The game over panel
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject scorePanel; // The game over panel
    [SerializeField] private int targetFPS = 60; // The target frames per second
    public UnityEvent gameoverEvent = new UnityEvent(); // The event to fire when the player is caught

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab; // The player prefab
    public TileGridCoords playerStartCoords = new(x: 5, z: 5); // The starting coordinates of the player
    public GameObject player; // The player object
    public PlayerMovement playerMovement; // The player movement script
    public PlayerController playerController; // The player controller script

    [Header("Map Settings")]
    [SerializeField] private TileManager tileManager; // The tile manager

    [Header("Speed Settings")]
    [SerializeField] private float _speed = 1; // backing field for Speed property (this is the actual speed of the game)
    [SerializeField] private float speedIncrement = 0.05f; // amount to increase the speed by as the game progresses
    [SerializeField] private float catchupSpeed = 8; // speed to catch up to the player
    [SerializeField] private float maxBaseSpeed = 4; // The maximum speed of the game independent of the catchup mechanic
    [SerializeField] private float accelerationCurve = 1.5f; // The speed at which the "camera" catches up to player (lower is faster)
    [SerializeField] private float decelerationCurve = 0.5f; // The speed at which the "camera" slows back down (lower is faster)

    private float baseSpeed; // The speed of the game independent of the catchup mechanic
    private float initSpeed = 1; // The initial speed  
    private float decelerationTolerance = 0.1f; // The tolerance for the speed to be considered back to normal
    private float currentVelocity = 0f;


    private bool isCatchingUp;
    public bool playerInCatchupZone = false;

    [Header("Crow settings")]
    [SerializeField] private CrowManager crowManager;

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
            _speed = Mathf.Clamp(value, 0, catchupSpeed);
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
        Application.targetFrameRate = targetFPS;
        initSpeed = Speed;
        gameoverEvent.AddListener(GameOver);
        Init();
    }

    void Update()
    {
        if (gameStarted == false) return;

        UpdateSpeed();
    }

    /// <summary>
    /// Initialise game state
    /// </summary>
    public void Init()
    {
        baseSpeed = Speed;
        GameoverPanel.SetActive(false);
        titlePanel.SetActive(true);
        scorePanel.SetActive(false);
        tileManager.InitTileGrid();
        SpawnPlayer();
    }

    public void StartGame()
    {
        crowManager.SpawnCrow();
        gameStarted = true;
        titlePanel.SetActive(false);
        scorePanel.SetActive(true);
    }

    /// <summary>
    /// Reset the game
    /// </summary>
    public void Reset()
    {
        Speed = initSpeed;
        isCatchingUp = false;
        playerInCatchupZone = false;
        score = 0;
        backMovements = 0;
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
        gameOver = true;
        // destroy player
        Destroy(player);
        GameoverPanel.SetActive(true);
        crowManager.GameOver();
    }

    /// <summary>
    /// Spawn the player at the starting coordinates
    /// </summary>
    private void SpawnPlayer()
    {
        if (player != null) { return; }
        player = tileManager.InstantiateOnTile(playerPrefab, playerStartCoords);
        playerMovement = player.GetComponent<PlayerMovement>();
        playerController = player.GetComponent<PlayerController>();
    }

    /// <summary>
    /// calculate the score based on the players movement.
    /// The function is called in the playermovement script.
    /// </summary>
    /// <param name="movement"></param>
    public void CalculateScore(Vector3 movement)
    {
        if (movement == Vector3.right) //checks if player is moving forward. (for some reason vector.right is forward)
        {
            if (backMovements > 0) //backmovements checks the amount of times the player has moved back without moving forward.
            {
                backMovements--;
            }
            else
            {
                score++; //increases the score by 1
            }
        }
        else if (movement == Vector3.left)
        {
            backMovements++;
        }
    }

    /// <summary>
    /// Update the speed of the game
    /// </summary>
    private void UpdateSpeed()
    {
        // Check if the player is in the catchup zone
        if (playerInCatchupZone)
        {
            // accelerate to catchup speed to catch up to the player
            Catchup();
        }
        else
        {
            // decelerate to the base speed
            StopCatchup();
        }

        // Until we reach the max base speed, increase the base speed
        if (baseSpeed < maxBaseSpeed)
        {
            baseSpeed += speedIncrement * Time.deltaTime;
        }

        // if not catching up, set the actual speed to the base speed
        if (isCatchingUp == false)
        {
            Speed = baseSpeed;
        }
    }

    /// <summary>
    /// Increase game speed to catch up to the player
    /// </summary>
    private void Catchup()
    {
        // accelerate to catchup speed
        Speed = Mathf.SmoothDamp(Speed, catchupSpeed, ref currentVelocity, accelerationCurve);

        isCatchingUp = true;
    }

    /// <summary>
    /// Stop catching up to the player
    /// </summary>
    private void StopCatchup()
    {
        // decelerate from catchup speed to the speed before catchup
        Speed = Mathf.SmoothDamp(Speed, baseSpeed, ref currentVelocity, decelerationCurve);

        // if we are close enough to the speed before catchup, stop catching up
        if (Mathf.Abs(Speed - baseSpeed) < decelerationTolerance)
        {
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