using System.Collections;
using UnityEngine;

public class CrowManager : MonoBehaviour
{
    [SerializeField] private GameObject crowModel;
    [SerializeField] private float moveDuration = 10f; //How fast the crow moves from inital X pos to end X pos
    private const int startXPos = 16; //Offscreen X location 'ahead' of the player
    private const int endXPos = -5; //Offscreen X location 'behind' the player
    private int zPos = 0;

    private void Start()
    {
        SetCrowLocation();
    }

    private void SetCrowLocation()
    {
        zPos = GetRandomZPos();
        Vector3 spawnPosition = new Vector3(startXPos, 0f, zPos);
        GameObject spawnedCrow = Instantiate(crowModel, spawnPosition, Quaternion.identity);
        StartCoroutine(CrowMove(spawnedCrow));
    }

    private IEnumerator CrowMove(GameObject crow)
    {
        float elapsedTime = 0f;
        Vector3 startPos = crow.transform.position;
        Vector3 endPos = new Vector3(endXPos, startPos.y, startPos.z);

        while (elapsedTime < 5f)
        {
            crow.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure crow reaches the exact position
        crow.transform.position = endPos;

        // Move to a new position and wait after reaching the end
        crow.transform.position = new Vector3(30f, 0f, 30f);
        yield return new WaitForSeconds(Random.Range(3f, 7f));

        zPos = GetRandomZPos(); 

        // Move back to start position and repeat
        crow.transform.position = new Vector3(startXPos, 0f, zPos);
        StartCoroutine(CrowMove(crow));
    }

    private int GetRandomZPos()
    {
        int newZPos = Random.Range(0, 10);
        return newZPos;
    }
}