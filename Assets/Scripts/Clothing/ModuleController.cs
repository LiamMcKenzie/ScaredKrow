/*
 * File: ModuleController.cs
 * Purpose: Holds data for a clothing module
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using UnityEngine;

/// <summary>
/// This class is used to hold data for a clothing module
/// It is created by the ModuleData scriptable object and attached to a module prefab in the module pool
/// </summary>
public class ModuleController : MonoBehaviour
{
    public ModuleType Type { get; private set; } // The type of module
    public string Style { get; private set; } // The style of the module

    /// <summary>
    /// Initialize the module controller with the module data
    /// </summary>
    public void Init(ModuleData data)
    {
        Type = data.Type;
        Style = data.Style;
    }
}