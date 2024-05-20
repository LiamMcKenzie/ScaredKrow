/*
 * File: MaterialOffsetSineController.cs
 * Purpose: Animates a material offset using a sine wave
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
/// This class is used to animate a material offset using a sine wave
/// </summary>
/// <remarks>
/// This is heavily geared towards animating the water texture in the game
/// But with the addition of flags for x and y offsets, it would be more versatile
/// </remarks>
public class MaterialOffsetSineController : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1.0f;

    private Vector2 offset = Vector2.zero;

    /// <summary>
    /// Every frame, apply a sine wave to the x offset and linear movement to the y offset
    /// </summary>
    private void Update()
    {
        // Apply linear movement to the y offset
        if (offset.y > 1.0f)
        {
            offset.y = 0.0f;
        }
        else
        {
            offset.y += speed * Time.deltaTime;
        }
        // Apply sine wave to the x offset
        offset.x = Mathf.Sin(frequency * Time.time) * amplitude;

        material.mainTextureOffset = new Vector2(offset.x, offset.y);
    }

    /// <summary>
    /// When the object is disabled or game closes, reset the offset
    /// </summary>
    private void OnDisable()
    {
        material.mainTextureOffset = Vector2.zero;
    }
}
