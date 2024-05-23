using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStartGame : MonoBehaviour
{
    


    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) )
        {
            StartGameplay();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartGameplay();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            StartGameplay();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            StartGameplay();
        }else if(Input.GetKeyDown(KeyCode.X)){ //I noticed some playtesters would try to start the game using the first arcade button (which is X key)
            StartGameplay();
        }
    }

    void StartGameplay()
    {
        GameManager.instance.StartGame();
        Destroy(gameObject);
    }
}
