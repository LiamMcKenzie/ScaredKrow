using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveDistance = 1f;
    public float moveTime = 0.2f;
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
        animator.Play("player_jump", 0, 0);
        //TODO: the jump animation is set at 0.2 seconds (12 frames), which is the same as the moveTime.
        //If we plan on changing the moveTime we will also need to change the animation length.

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
