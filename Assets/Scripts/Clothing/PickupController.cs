using UnityEngine;
using System;

public class PickupController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private ModuleType type;
    private const float Y_OFFSET = -0.2f;
    private Quaternion HAT_ROTATION = Quaternion.Euler(-90, 180, 0);
    private Quaternion SHIRT_ROTATION = Quaternion.Euler(-90, 0, 0);
    private Quaternion PANTS_ROTATION = Quaternion.Euler(-90, 90, 0);

    public void SetPickupType()
    {
        transform.localPosition = new Vector3(0, Y_OFFSET, 0);
        type = (ModuleType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ModuleType)).Length);

        // switch case to set the rotation and position of the clothing item
        // switch case to set the rotation and position of the clothing item
        switch (type)
        {
            case ModuleType.Head:
                transform.localRotation = HAT_ROTATION;
                break;
            case ModuleType.Torso:
                transform.localRotation = SHIRT_ROTATION;
                break;
            case ModuleType.Legs:
                transform.localRotation = PANTS_ROTATION;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.transform.parent.GetComponent<PlayerController>();
            player.PickupModule(type);   
            
            AudioManager.PlaySound(7, 2f); //pickup sound
            Destroy(gameObject);
        }
    }
}
