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

        if(audioSource != null)
        {
            audioSource = GetComponent<AudioSource>();
        }else{
            Debug.LogWarning("Audio source not assigned to audio manager.");
        }
    }

    private void onPlaySound(int soundIndex, float volume)
    {
        Debug.Log("Playing sound: " + audioClips[soundIndex].name);
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
    /// <param name="soundIndex">The index of the sound to play</param>
    /// <param name="volume">The volume of the sound</param>

    //Example: AudioManager.PlaySound(0); //plays jump sound    
    public static void PlaySound(int soundIndex, float volume)
    {
        
        if (Instance != null)
        {
            Instance.onPlaySound(soundIndex, volume);
        }else
        {
            Debug.LogWarning("Audio manager not assigned.");
        }
    }
}
