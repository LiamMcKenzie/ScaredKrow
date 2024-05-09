using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls the slow forward movement of the camera.
/// 
/// This script should be applied to an empty (0, 0, 0) parent object, with the camera child having the rotation and specific position values.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    public float moveSpeed;
    public bool moving = true;

    void Update()
    {
        if(moving == true) //This can be set to false later on, since bushes are planned to stop the camera speed.
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World); //The camera will slowly move forward on the x axis, vector3.right == (1, 0, 0)
        }           
    }
}
