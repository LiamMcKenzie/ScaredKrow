/*
 * File: CrowManager.cs
 * Purpose: Handles the spawning and moving of a crow prefab during gameplay
 *          Shows a player alert when the crow spawns and starts moving
 *          
 * Author: Devon
 * Contributuions: Some assistance from ChatGPT in the Crow position Vector3.Lerp
 * 
 * Notes:  - Currently the crow speed is calculated as 'time taken to reach the endpoint' instead of a speed value. This means to make the crow faster
 *           we would need to decrease this value over time, to a minimum of 3sec travel time (from 5sec). 
 *           Could change this to be a speed value that gradually increases instead if required though.
 *        
 *         - Not sure if starting the coroutine from within itself is best practice ( end of MoveCrow() )
 */

using System.Collections;
using UnityEngine;

public class CrowManager : MonoBehaviour
{
    [Header("Main Game Objects")]
    [SerializeField] private GameManager gameManager; //GameManager script for gameStarted check
    [SerializeField] public PlayerController playerController; //PlayerController script for player alert

    [Header("Prefab Gameobjects")]
    [SerializeField] private GameObject crowModel; //CrowEnemy prefab for the crow


    [Header("Crow Movement settings")]
    [SerializeField] private float travelTime = 5f; //How fast the crow moves from inital X pos to end X pos (in seconds)
    [SerializeField] private float minSpawnDelay = 2f; //Minimum time (in seconds) before 'respawning' crow in player path
    [SerializeField] private float maxSpawnDelay = 5f; //Maximum time (in seconds) before 'respawning' crow in player path
    private Vector3 crowPosition; //Spawn position for crow
    private GameObject spawnedCrow; //Stores Crow gameobject instantiated in SpawnCrow() for movement

    [Header("Constant/Initial movement axis values")]
    private const int startXPos = 16; //Offscreen X location 'ahead' of the player
    private const int endXPos = -5; //Offscreen X location 'behind' the player
    private int zPos = 0; //Initial z-axis location before updating with random value

    void Start()
    {
        gameManager = GameManager.instance;
        playerController = gameManager.playerController;
    }

    /// <summary>
    /// Instantiates a Crow gameobject at a position offscreen (x-axis) at a random point on the z-axis
    /// Start Coroutine to move the crow
    /// </summary>
    public void SpawnCrow()
    {
        if (GameManager.instance.gameStarted == false) { return; }

        zPos = GetRandomZPos();
        crowPosition = new Vector3(startXPos, 0f, zPos);

        if (spawnedCrow == null)
        {
            spawnedCrow = Instantiate(crowModel, crowPosition, Quaternion.identity);
        }
        else
        {
            spawnedCrow.transform.position = crowPosition; //Reset position if already spawned
        }

        StartCoroutine(MoveCrow(spawnedCrow));
    }

    /// <summary>
    /// Moves a crow gameobject along the x-axis towards an end point
    /// Waits before moving the crow again at a new random z-axis position
    /// </summary>
    /// <param name="crow">Instantiated crow gameobject spawned in the scene</param>
    /// <returns>WaitForSeconds to delay Crow respawning</returns>
    private IEnumerator MoveCrow(GameObject crow)
    {
        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay)); //Wait a random amount before 'respawning'
        //Show an alert when the crow spawns
        playerController.AlertActive();

        //Set values for time and start/end positions
        float elapsedTime = 0f;
        Vector3 startPos = crow.transform.position;
        Vector3 endPos = new Vector3(endXPos, startPos.y, startPos.z);

        //Move the crow over time from a start to end point
        while (elapsedTime < travelTime)
        {
            crow.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move to a new position offscreen and wait (after reaching the end)
        crow.transform.position = new Vector3(30f, 0f, 30f); //Offscreen location
        

        SpawnCrow(); //Respawn crow at new location and move again
    }


    /// <summary>
    /// Sets a random z-axis starting position within player move space
    /// </summary>
    /// <returns>z position for new crow spawn location</returns>
    private int GetRandomZPos() => Random.Range(2, 9);
}