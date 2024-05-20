/*
 * File: GameoverTriggerArea.cs
 * Purpose: Trigger area for player being caught by "camera"
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;
using UnityEngine.Events;

/// <summary>
// This class controls a trigger area for the Gameover event.
/// </summary>
public class GameoverTriggerArea : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; // The game manager
    
    /// <summary>
    /// The trigger area for gameover is entered
    /// Thsi should be the bottom of the playfield
    /// </summary>
    /// <param name="other">The collider that entered the trigger area, probably the player</param>
    void OnTriggerEnter(Collider other)
    {
        // check other is player
        if (other.gameObject.CompareTag("Player") == false) return;

        // fire event
        gameManager.gameoverEvent.Invoke();
    }
}
