using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Crow collided with the player!");
            //Add more functionality later after collision (i.e damaging player)
        }
    }
}
