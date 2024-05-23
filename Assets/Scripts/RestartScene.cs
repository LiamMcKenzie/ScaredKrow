using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartScene : MonoBehaviour
{
    private void Update()
    {
        if(GameManager.instance.gameOver == true)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
            {
                Restart();
            }
        }
        
    }

    public void Restart()
    {
        // Add your restart scene logic here
        // For example, you can reload the current scene using SceneManager.LoadScene()
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
