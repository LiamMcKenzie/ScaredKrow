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
    [Header("Prefab Gameobjects")]
    [SerializeField] private GameObject crowModel; //CrowEnemy prefab for the crow
    [SerializeField] public GameObject playerAlert; //Alert prefab attached to the player
    private bool alertFound = false;

    [Header("Crow Movement settings")]
    [SerializeField] private float travelTime = 5f; //How fast the crow moves from inital X pos to end X pos (in seconds)
    [SerializeField] private float minSpawnDelay = 2f; //Minimum time (in seconds) before 'respawning' crow in player path
    [SerializeField] private float maxSpawnDelay = 5f; //Maximum time (in seconds) before 'respawning' crow in player path

    [Header("Constant/Initial movement axis values")]
    private const int startXPos = 16; //Offscreen X location 'ahead' of the player
    private const int endXPos = -5; //Offscreen X location 'behind' the player
    private int zPos = 0; //Initial z-axis location before updating with random value

    /// <summary>
    /// Spawn an initial crow at a location
    /// </summary>
    private void Start()
    {
        SpawnCrow();
    }

    /// <summary>
    /// Instantiates a Crow gameobject at a position offscreen (x-axis) at a random point on the z-axis
    /// Start Coroutine to move the crow
    /// </summary>
    private void SpawnCrow()
    {
        zPos = GetRandomZPos();
        Vector3 spawnPosition = new Vector3(startXPos, 0f, zPos);
        GameObject spawnedCrow = Instantiate(crowModel, spawnPosition, Quaternion.identity);
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
        //Show an alert when the crow spawns
        StartCoroutine(ShowAlert()); 

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
        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay)); //Wait a random amount before 'respawning'

        //Get a new z-axiz position for the crow to move along
        zPos = GetRandomZPos(); 

        // Move crow to new start position and repeat movement coroutine
        crow.transform.position = new Vector3(startXPos, 0f, zPos);
        StartCoroutine(MoveCrow(crow));
    }

    /// <summary>
    /// Toggles the alert above the player to be active for half the crow travel time
    /// </summary>
    /// <returns>WaitForSeconds before hiding gameobject again</returns>
    private IEnumerator ShowAlert()
    {
        //Initial reference to alert gameobject
        if (!alertFound) 
        { 
            playerAlert = GameObject.FindWithTag("Alert"); 
            alertFound = true;
        }

        //Toggle the alert on/off
        playerAlert.SetActive(true);
        yield return new WaitForSeconds(travelTime / 2f);
        playerAlert.SetActive(false);
    }

    /// <summary>
    /// Sets a random z-axis starting position within player move space
    /// </summary>
    /// <returns>z position for new crow spawn location</returns>
    private int GetRandomZPos() => Random.Range(1, 9);
}