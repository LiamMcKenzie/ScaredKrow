using UnityEngine;

public class CrowSpawn : MonoBehaviour
{
    [SerializeField] private GameObject crowModel; // The GameObject to spawn
    [SerializeField] private int spawnXCoordinate = 16; // The x coordinate to spawn the object
    [SerializeField] private float tileSize = 1f; // The size of each tile
    [SerializeField] private int objectWidthInTiles = 3; // The width of the object in tiles

    private void Start()
    {
        SpawnRandomObject();
    }

    private void SpawnRandomObject()
    {
        // Choose a random z coordinate within the range of the grid
        int randomZCoordinate = Random.Range(0, 10);

        // Calculate the world position based on the chosen coordinates
        Vector3 spawnPosition = new Vector3(spawnXCoordinate * tileSize, 0f, randomZCoordinate * tileSize);

        // Spawn the object at the calculated position
        Instantiate(crowModel, spawnPosition, Quaternion.identity);
    }
}