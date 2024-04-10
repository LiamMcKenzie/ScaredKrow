using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveTime = 0.5f;
    private bool isMoving = false;


    void Update()
    {
        //This is obviously not a great way of doing this.
        //This could be changed with the new input system but I just wanted to quickly test this.
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.right * moveDistance));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.left * moveDistance));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.forward * moveDistance));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.back * moveDistance));
        }
    }

    IEnumerator MovePlayer(Vector3 movement)
    {
        isMoving = true;
        Vector3 startPosition = transform.position; 
        Vector3 endPosition = startPosition + movement;

        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        isMoving = false;
    }
}
