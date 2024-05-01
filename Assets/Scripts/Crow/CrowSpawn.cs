using UnityEngine;

public class CrowSpawn : MonoBehaviour
{
    [SerializeField] private GameObject crowModel; // The GameObject to spawn
    [SerializeField] private const int spawnXCoordinate = 16; // The x coordinate to spawn the object

    private void Start()
    {
        SetCrowLocation();
    }

    private void SetCrowLocation()
    {
        // Choose a random z coordinate within the range of the grid
        int randomZCoordinate = Random.Range(0, 10);

        // Calculate the world position based on the chosen coordinates
        Vector3 spawnPosition = new Vector3(spawnXCoordinate, 0f, randomZCoordinate);

        // Spawn the object at the calculated position
        Instantiate(crowModel, spawnPosition, Quaternion.identity);
    }
}