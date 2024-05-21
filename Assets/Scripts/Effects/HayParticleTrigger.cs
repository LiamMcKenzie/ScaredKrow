using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayParticleTrigger : MonoBehaviour
{
    public ParticleSystem hayParticles;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            hayParticles.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            hayParticles.Stop();
        }
    }
}
