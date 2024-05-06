/*
 * File: CrowManager.cs
 * Purpose: Handles the spawning and moving of a crow prefab during gameplay
 *          Calculates shadow colliding with player for alerts/hit
 * Author: Devon
 * Contributuions: Some assistance from ChatGPT in getting shadow collision accurate to directional light rotation
 * 
 * Notes: Currently the crow speed is calculated as 'time taken to reach the endpoint' instead of a speed value. This means to make the crow faster
 *        we would need to decrease this value over time, to a minimum of 2sec travel time (from 10sec). Could change this to be a speed value that gradually increases
 *        instead if required though.
 *        
 *        Not sure if starting the coroutine from within itself is best practice ( end of MoveCrow() )
 */

using System.Collections;
using UnityEngine;

public class CrowManager : MonoBehaviour
{
    [SerializeField] private GameObject crowModel;
    [SerializeField] private GameObject playerAlert;
    [SerializeField] private float travelTime = 5f; //How fast the crow moves from inital X pos to end X pos (in seconds)
    [SerializeField] private float minSpawnDelay = 2f;
    [SerializeField] private float maxSpawnDelay = 5f;
    private const int startXPos = 16; //Offscreen X location 'ahead' of the player
    private const int endXPos = -5; //Offscreen X location 'behind' the player
    private int zPos = 0;

    private void Start()
    {
        SpawnCrow();
    }

    private void SpawnCrow()
    {
        zPos = GetRandomZPos();
        Vector3 spawnPosition = new Vector3(startXPos, 0f, zPos);
        GameObject spawnedCrow = Instantiate(crowModel, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveCrow(spawnedCrow));
    }

    private IEnumerator MoveCrow(GameObject crow)
    {
        StartCoroutine(ShowAlert());

        float elapsedTime = 0f;
        Vector3 startPos = crow.transform.position;
        Vector3 endPos = new Vector3(endXPos, startPos.y, startPos.z);

        while (elapsedTime < travelTime)
        {
            crow.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move to a new position and wait after reaching the end
        crow.transform.position = new Vector3(30f, 0f, 30f); //Hide crow offscreen
        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

        zPos = GetRandomZPos(); 

        // Move back to start position and repeat
        crow.transform.position = new Vector3(startXPos, 0f, zPos);
        StartCoroutine(MoveCrow(crow));
    }

    private IEnumerator ShowAlert()
    {
        playerAlert.SetActive(true);
        yield return new WaitForSeconds(2f);
        playerAlert.SetActive(false);
    }

    /// <summary>
    /// Sets a random z-axis starting position within player move space
    /// </summary>
    /// <returns>z position for new crow spawn location</returns>
    private int GetRandomZPos() => Random.Range(0, 10);
}