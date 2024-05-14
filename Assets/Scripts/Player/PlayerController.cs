/*
 * File: PlayerController.cs
 * Purpose: Manage player state
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;
using System.Collections;
using UnityEditor.Experimental;

/// <summary>
/// This class is used to control the player state
/// It will be used to manage the player's health and pickups
/// </summary>
public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement playerMovement;
    public GameObject alertObject;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = GameManager.instance;
    }

    /// <summary>
    /// Log some debug information about the player
    /// </summary>
    [ContextMenu("Log Info")]
    void LogInfo()
    {
        Debug.Log("Transform position: " + transform.position);
        Debug.Log("Grid Position: " + playerMovement.gridPosition);
        if (transform.parent != null)
        {
            Debug.Log("Parent Position: " + transform.parent.position);
            Debug.Log("Parent Name: " + transform.parent.name);
        }
        else
        {
            Debug.Log("This object does not have a parent.");
        }
    }

    [ContextMenu("Show Alert")]
    public void AlertActive(float travelTime = 5f)
    {
        StartCoroutine(ShowAlert(travelTime));
    }


    [ContextMenu("Hide Alert")]
    private IEnumerator ShowAlert(float travelTime)
    {
        //Toggle the alert on/off
        alertObject.SetActive(true);
        yield return new WaitForSeconds(travelTime / 2f);
        alertObject.SetActive(false);
    }

    /// <summary>
    /// Move player object to root level
    /// </summary>
    [ContextMenu("Move To Root")]
    public void MoveToRoot()
    {
        Debug.Log("Moving player to root level");
        transform.parent = null;
        // turn off player movement
        playerMovement.enabled = false;

    }

    public void ResetPlayer()
    {
        playerMovement.enabled = true;
        playerMovement.ResetPlayer();
    }

}
