/*
 * File: ModuleData.cs
 * Purpose: Scriptable bject for a clothing module
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

[CreateAssetMenu(fileName = "ModuleData")]
public class ModuleData : ScriptableObject
{
    [field: SerializeField] public ModuleType Type { get; private set; }
    [field: SerializeField] public string Style { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    /// <summary>
    /// Create a module from the prefab and inject the module data
    /// </summary>
    /// <returns>The module controller</returns>
    public ModuleController CreateModule()
    {
        GameObject module = Instantiate(Prefab);
        module.AddComponent<ModuleController>().Init(this);
        return module.GetComponent<ModuleController>();
    }
}
