/*
 * File: CatchupArea.cs
 * Purpose: Trigger area for "camera" to catch up to player
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
// This class controls a trigger area for the catchup function of the Game Manager. 
// When the player enters this area, the game speed increases to move the player back out of the trigger
/// </summary>
public class CatchupArea : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; // The player object

    /// <summary>
    /// The trigger area for the camera to catch up to the player is entered
    /// </summary>
    /// <param name="other">The collider that entered the trigger area, probably the player</param>
    void OnTriggerEnter(Collider other)
    {
        // check other is player
        if (other.gameObject.CompareTag("Player") == false) return;

        // start catchup in the game manager
        gameManager.playerInCatchupZone = true;
    }

    /// <summary>
    /// The trigger area for the camera to catch up to the player is exited
    /// </summary>
    /// <param name="other">The collider that entered the trigger area, probably the player</param>
    void OnTriggerExit(Collider other)
    {
        // check other is player
        if (other.gameObject.CompareTag("Player") == false) return;

        // stop catchup in the game manager
        gameManager.playerInCatchupZone = false;
    }
}
