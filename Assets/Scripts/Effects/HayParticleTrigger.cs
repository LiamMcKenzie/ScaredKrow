/*
 * File: HayParticleTrigger.cs
 * Purpose: Trigger area for starting/stopping haybale particles on player collision
 * Author: Devon
 */

using UnityEngine;

public class HayParticleTrigger : MonoBehaviour
{
    public ParticleSystem hayParticles;

    /// <summary>
    /// Start the particle system
    /// </summary>
    /// <param name="other">Collision object collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            hayParticles.Play();
            AudioManager.PlaySound(1);
        }
    }

    /// <summary>
    /// Stop the particle system
    /// </summary>
    /// <param name="other">Collision object collider</param>
    private void OnTriggerExit(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            hayParticles.Stop();
        }
    }
}
