using System.Collections;
using UnityEngine;


/// <summary>
/// Controls the player movement, and player grid position
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveDistance = 1f;
    public float moveTime = 0.2f; //Time in seconds
    private bool isMoving = false;
    public TileGridCoords gridPosition; 

    private Vector3 bufferedDirection = Vector3.zero;

    void Start()
    {
        gridPosition = GameManager.instance.playerStartCoords;
        TileManager.instance.GridRepositioned.AddListener(OnGridRepositioned);
    }

    void OnGridRepositioned()
    {
        gridPosition.x -= TileManager.instance.tilesHigh;
    }

    void Update()
    {
        //This is obviously not a great way of doing this.
        //This could be changed with the new input system but I just wanted to quickly test this.


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) )
        {
            BufferDirection(Vector3.right * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            BufferDirection(Vector3.left * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            BufferDirection(Vector3.forward * moveDistance);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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

        animator.Play("player_jump", 0, 0); //plays the jump animation
        
        /*
        if(TileManager.instance.masterTileControllerList[desiredGridPosition.x][desiredGridPosition.z].isPassable == false) //checks if the desired tile is not passable.  
        {
            yield break; //exits this IEnumerator. if the desired tile is passable then all the code below runs
        }*/

        isMoving = true; 
        TileController currentTile = TileManager.instance.masterTileControllerList[gridPosition.x][gridPosition.z]; //gets the current tile
        TileController desiredTile = TileManager.instance.masterTileControllerList[desiredGridPosition.x][desiredGridPosition.z]; //gets the desired tile
        bool canMove = desiredTile.isPassable; //checks if the desired tile is passable and not a boundary

        if(canMove)
        {
            currentTile.containsPlayer = false; //removes the player from the current tile
            desiredTile.PlayerEntersTile(transform); //sets he players parent
            gridPosition = desiredGridPosition; //updates the grid position to the desired position
        }

        //after the player is assigned a new parent, its position relative to the parent will be off by 1 unit roughly.
        Vector3 startPosition = transform.localPosition; //relative to the new parent, which has just been set above
        Vector3 endPosition = Vector3.zero; //0,0,0 is perfectly center with the new parent. 

        float elapsedTime = 0;

        //TODO: the jump animation is set at 0.2 seconds (12 frames), which is the same as the moveTime.
        //If we plan on changing the moveTime we will also need to change the animation length.

        //smoothly moves/rotates the player towards the desired position/rotation
        while (elapsedTime < moveTime)
        {
            if(canMove)
            {
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));

            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}
