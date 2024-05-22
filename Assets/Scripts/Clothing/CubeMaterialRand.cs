using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeMaterialRand : MonoBehaviour
{

    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            int materialIndex = Random.Range(0, materials.Length);
            renderer.material = materials[materialIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
