/*
 * File: CrowCollsion.cs
 * Purpose: Checks if the Crows 'shadow' collider has hit the player    
 * Author: Devon
 * Notes:  - Just a very basic implementation at this stage (Debug log). 
 *         - Will have more functionality of decreasing the players health when we have that added
 */
using UnityEngine;

public class CrowCollision : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            GameManager.instance.gameoverEvent.Invoke();
            Debug.Log("Crow collided with the player!");
            //Add more functionality later (i.e damaging player)
        }
    }
}
