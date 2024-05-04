using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveDistance = 1f;
    public float moveTime = 0.2f;
    private bool isMoving = false;
    public TileGridCoords gridPosition; 

    private Vector3 bufferedDirection = Vector3.zero;

    void Start()
    {
        gridPosition = GameManager.instance.playerStartCoords;
    }

    void Update()
    {
        //This is obviously not a great way of doing this.
        //This could be changed with the new input system but I just wanted to quickly test this.

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BufferDirection(Vector3.right * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BufferDirection(Vector3.left * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BufferDirection(Vector3.forward * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            BufferDirection(Vector3.back * moveDistance);
        }

        //check if we are not moving and there is a buffered direction
        if (!isMoving && bufferedDirection != Vector3.zero)
        {
            StartCoroutine(MovePlayer(bufferedDirection));
            bufferedDirection = Vector3.zero; //reset buffered direction  
        }
    }

    void BufferDirection(Vector3 direction)
    {
        if (!isMoving)
        {
            StartCoroutine(MovePlayer(direction));  //player isn't moving so move normally
        }
        else
        {
            bufferedDirection = direction;  //player is moving so buffer the direction
        }
    }

    IEnumerator MovePlayer(Vector3 movement)
    {
        //tile the player wants to move to.
        TileGridCoords desiredGridPosition = new(gridPosition.x + Mathf.FloorToInt(movement.x), gridPosition.z + Mathf.FloorToInt(movement.z));

        if(TileManager.instance.masterTileControllerList[desiredGridPosition.x][desiredGridPosition.z].isPassable == false)
        {
            yield break;
        }

        isMoving = true; 

        gameObject.transform.SetParent(TileManager.instance.masterTileControllerList[desiredGridPosition.x][desiredGridPosition.z].gameObject.transform);
        gridPosition = desiredGridPosition;

        Vector3 startPosition = transform.localPosition; 
        Vector3 endPosition = Vector3.zero;

        float elapsedTime = 0;
        animator.Play("player_jump", 0, 0);

        //TODO: the jump animation is set at 0.2 seconds (12 frames), which is the same as the moveTime.
        //If we plan on changing the moveTime we will also need to change the animation length.

        while (elapsedTime < moveTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPosition;

        isMoving = false;
    }
}
