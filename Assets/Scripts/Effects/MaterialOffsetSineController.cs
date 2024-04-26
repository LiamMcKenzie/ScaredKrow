using UnityEngine;

public class MaterialOffsetSineController : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1.0f;

    private Vector2 offset = Vector2.zero;

    private void Update()
    {
        if (offset.y > 1.0f)
        {
            offset.y = 0.0f;
        }
        else
        {
            offset.y += speed * Time.deltaTime;
        }
        offset.x = Mathf.Sin(frequency * Time.time) * amplitude;

        material.mainTextureOffset = new Vector2(offset.x, offset.y);
    }

    private void OnDisable()
    {
        material.mainTextureOffset = Vector2.zero;
    }
}
