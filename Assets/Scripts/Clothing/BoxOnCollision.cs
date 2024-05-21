using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOnCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Box collided with the player!");
            //Add more functionality later (i.e damaging player)
        }
    }
}
