using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyLoad : MonoBehaviour
{
    void Awake()
    {
        //This doesn't seem to work idk why. the audio is just gonna restart i guess
        //its supposed to keep the audio playing when you change scenes.
        DontDestroyOnLoad(gameObject);
        //Debug.Log("DontDestroyOnLoad: " + this.gameObject.name);
    }
}
