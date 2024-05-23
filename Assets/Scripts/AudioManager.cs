using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <remarks>
/// This is a static class which provides easy access for playing sounds from other scripts.
/// To use call this function: AudioManager.PlaySound(int soundIndex)
/// </remarks>

/// <summary>
/// This script manages audio playback.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [HideInInspector] //This is assigned automatically
    public AudioSource audioSource;

    public AudioClip[] audioClips;      //Assign audio clips in editor

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void onPlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < audioClips.Length)
        {
            audioSource.PlayOneShot(audioClips[soundIndex]);
        }
        else
        {
            Debug.LogWarning("Sound index out of range");
        }
    }

    /// <summary>
    /// Plays a sound from the audio manager.
    /// Requests a sound index to play.
    /// </summary>
    /// <param name="soundIndex"></param>
    public static void PlaySound(int soundIndex)
    {
        if (Instance != null)
        {
            Instance.onPlaySound(soundIndex);
        }
    }
}
