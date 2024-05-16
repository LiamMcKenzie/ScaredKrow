/*
 * File: PlayerController.cs
 * Purpose: Manage player state
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to control the player state
/// It will be used to manage the player's health and pickups
/// </summary>
public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement playerMovement;
    [SerializeField] GameObject playerAlert;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = GameManager.instance;
    }

    /// <summary>
    /// Toggles the alert above the player to be active for half the crow travel time
    /// </summary>
    /// <returns>WaitForSeconds before hiding gameobject again</returns>
    private IEnumerator AlertOnOff()
    {
        //Toggle the alert on/off
        playerAlert.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        playerAlert.SetActive(false);
    }

    /// <summary>
    /// Run the alert coroutine
    /// </summary>
    public void ShowAlert()
    {
        StartCoroutine(AlertOnOff());
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
