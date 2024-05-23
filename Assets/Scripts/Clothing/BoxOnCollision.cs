using UnityEngine;
using System;

public class BoxOnCollision : MonoBehaviour
{
    private ClothingType type;
    void Start()
    {
        // shirt rotation -90 0 0
        // pants rotation -90 90 0
        // hat rotation 0 -90 0 + pos 0.61 0.3 0
        //select a random clothing type
        type  = (ClothingType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ClothingType)).Length);

        // switch case to set the rotation and position of the clothing item
        switch (type)
        {
            case ClothingType.Hat:
                transform.Rotate(0, -90, 0);
                transform.position = new Vector3(0.61f, 0.3f, 0);
                break;
            case ClothingType.Shirt:
                transform.Rotate(-90, 0, 0);
                break;
            case ClothingType.Pants:
                transform.Rotate(-90, 90, 0);
                break;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Box collided with the player!");
            //Add more functionality later (i.e damaging player)
            Destroy(gameObject);
        }
    }
}
