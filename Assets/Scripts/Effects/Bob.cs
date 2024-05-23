/*
 * File: Bob.cs
 * Purpose: Simple bobbing effect for an object
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */
using UnityEngine;

public class Bob : MonoBehaviour
{
    public float bobSpeed = 1;
    public float bobHeight = 0.2f;
    public float bobWidth = 0.2f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        float newZ = Mathf.Sin(Time.time * bobSpeed) * bobWidth;
        transform.position = new Vector3(transform.position.x, startPos.y + newY, startPos.z + newZ);
    }
}