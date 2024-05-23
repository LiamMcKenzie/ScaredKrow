/*
 * File: OutfitController.cs
 * Purpose: Swaps in and out clothing modules
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ModuleType { Head, Torso, Legs }

/// <summary>
/// This struct is used to hold the parent object for a module and a reference to the base module
/// </summary>
[Serializable]
public struct ModuleParent
{
    [field: SerializeField] public ModuleType Type { get; private set; }
    [field: SerializeField] public GameObject Parent { get; private set; }
    [field: SerializeField] public GameObject BaseModule { get; private set; }
}

/// <summary>
/// This class maintains a reference to the module controller currently in the parent object
/// </summary>
public class ModuleInfo
{
    public ModuleParent ModuleParent { get; private set; }
    public ModuleController CurrentAdornment { get; set; }
    public bool IsAdorned => CurrentAdornment != null;

    public ModuleInfo(ModuleParent moduleParent)
    {
        ModuleParent = moduleParent;
        CurrentAdornment = null;
    }
}

/// <summary>
/// This class is used to control the player's outfit
/// It works in tandem with the ModuleManager to swap in and out clothing modules
/// </summary>
public class OutfitController : MonoBehaviour
{
    [SerializeField] private List<ModuleParent> moduleParents = new();
    private ModuleManager moduleManager;
    private List<ModuleInfo> moduleInfos = new(); // Assign the parent objects and base modules in the inspector


    private List<ModuleInfo> CurrentOutfit => moduleInfos.Where(info => info.IsAdorned).ToList();
    public bool IsNude => CurrentOutfit.Count == 0;
    private ModuleInfo GetModuleInfo(ModuleType type) => moduleInfos.First(info => info.ModuleParent.Type == type);

    void Start()
    {
        moduleManager = ModuleManager.Instance;
        foreach (var moduleParent in moduleParents)
        {
            moduleInfos.Add(new ModuleInfo(moduleParent));
        }
    }

    /// <summary>
    /// Set the module of the specified type
    /// </summary>
    /// <param name="type">The type of module to set</param>
    public void SetModule(ModuleType type)
    {
        // Get the module slot info
        var moduleInfo = GetModuleInfo(type);
        var moduleParent = moduleInfo.ModuleParent;

        // Deactivate the base module
        moduleParent.BaseModule.SetActive(false);

        // Get the module from ModuleManager
        var module = moduleManager.GetModule(type);

        // Return the currently adorned module to ModuleManager if there is one
        if (moduleInfo.IsAdorned)
        {
            moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
        }

        // Store a reference to the new module in the module info
        moduleInfo.CurrentAdornment = module;

        // Set the module's parent and position
        module.transform.SetParent(moduleParent.Parent.transform);
        module.transform.localPosition = Vector3.zero;
        module.transform.localRotation = moduleParent.BaseModule.transform.localRotation;
    }

    /// <summary>
    /// Remove a random module from the outfit
    /// </summary>
    public void RemoveModule()
    {
        if (IsNude) { return; }
        var moduleInfo = CurrentOutfit[UnityEngine.Random.Range(0, CurrentOutfit.Count)];
        moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
        moduleInfo.CurrentAdornment = null;
        moduleInfo.ModuleParent.BaseModule.SetActive(true);
    }

    /// <summary>
    /// Return all adorned modules to the ModuleManager when OutfitController is destroyed; ie when the player dies
    /// </summary>
    void OnDestroy()
    {
        if (moduleManager == null) { return; }
        foreach (var moduleInfo in moduleInfos)
        {
            if (moduleInfo.IsAdorned)
            {
                moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
            }
        }
    }

    [ContextMenu("Log Outfit")]
    private void LogOutfit()
    {
        foreach (var moduleInfo in moduleInfos)
        {
            Debug.Log(moduleInfo.ModuleParent.Type + " is adorned: " + moduleInfo.IsAdorned);
            Debug.Log("Current Outfit Count: " + CurrentOutfit.Count);
            Debug.Log("Is Nude: " + IsNude);
        }
    }
}