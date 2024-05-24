using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverTimer : MonoBehaviour
{
    public Image fillImage; // Reference to the image component
    
    void Start()
    {
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        fillImage.fillAmount =  GameManager.instance.timeOnGameover / GameManager.instance.timeToRestart;
    }
}
