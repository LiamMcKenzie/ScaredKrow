/*
 * File: CrowCollsion.cs
 * Purpose: Checks if the Crows 'shadow' collider has hit the player
 *          Inflicts damage to player on collision
 * Author: Devon
 */

using UnityEngine;

public class CrowCollision : MonoBehaviour
{
    private PlayerController player;
    PlayerMovement playerMovement;

    /// <summary>
    /// Checks if the colliding object is the player and infilicts damage
    /// </summary>
    /// <param name="other">Collision object collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            

            if (player == null) 
            {
                //Get the player health script if not referenced already
                player = other.transform.parent.GetComponent<PlayerController>();
                playerMovement = player.playerMovement;
            } 
         
            if (!playerMovement.isHiding)
            {
                AudioManager.PlaySound(6, 10f); //hit sound
                player.TakeHit();
            }
        }
    }
}
