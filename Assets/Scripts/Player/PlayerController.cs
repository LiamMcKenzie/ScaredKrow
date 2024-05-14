/*
 * File: PlayerController.cs
 * Purpose: Manage player state
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
/// This class is used to control the player state
/// It will be used to manage the player's health and pickups
/// </summary>
public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement playerMovement;

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
}
