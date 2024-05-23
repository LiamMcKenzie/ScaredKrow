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
public struct ModuleSlot
{
    [field: SerializeField] public ModuleType Type { get; private set; }
    [field: SerializeField] public GameObject Parent { get; private set; }
    [field: SerializeField] public GameObject BaseModule { get; private set; }
}

/// <summary>
/// This class maintains a reference to the module controller currently in a slot
/// </summary>
public class ModuleSlotInfo
{
    public ModuleSlot ModuleSlot { get; private set; }
    public ModuleController CurrentAdornment { get; set; }
    public bool IsAdorned => CurrentAdornment != null;

    public ModuleSlotInfo(ModuleSlot moduleParent)
    {
        ModuleSlot = moduleParent;
        CurrentAdornment = null;
    }
}

/// <summary>
/// This class is used to control the player's outfit
/// It works in tandem with the ModuleManager to swap in and out clothing modules
/// </summary>
public class OutfitController : MonoBehaviour
{
    private ModuleManager moduleManager;
    public bool IsNude => CurrentOutfit.Count == 0;
    [SerializeField] private List<ModuleSlot> moduleSlots = new(); // Assign the parent objects and base modules in the inspector
    private List<ModuleSlotInfo> moduleSlotInfos = new();
    private List<ModuleSlotInfo> CurrentOutfit => moduleSlotInfos.Where(info => info.IsAdorned).ToList();
    private ModuleSlotInfo GetModuleSlotInfo(ModuleType type) => moduleSlotInfos.First(info => info.ModuleSlot.Type == type);

    void Start()
    {
        moduleManager = ModuleManager.Instance;
        foreach (var moduleParent in moduleSlots)
        {
            moduleSlotInfos.Add(new ModuleSlotInfo(moduleParent));
        }
    }

    /// <summary>
    /// Set the module of the specified type
    /// </summary>
    /// <param name="type">The type of module to set</param>
    public void SetModule(ModuleType type)
    {
        // Get the module slot info
        var moduleInfo = GetModuleSlotInfo(type);
        var moduleParent = moduleInfo.ModuleSlot;

        // Deactivate the base module
        moduleParent.BaseModule.SetActive(false);

        // Get the module from ModuleManager
        var module = moduleManager.GetModule(type);

        // Return the currently adorned module to ModuleManager if there is one
        if (moduleInfo.IsAdorned)
        {
            moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
        }

        // Store a reference to the new module in the slot info
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
        moduleInfo.ModuleSlot.BaseModule.SetActive(true);
    }

    /// <summary>
    /// Return all adorned modules to the ModuleManager when OutfitController is destroyed; ie when the player dies
    /// </summary>
    void OnDestroy()
    {
        // null check
        if (moduleManager == null) { return; }
        foreach (var moduleInfo in moduleSlotInfos)
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
        foreach (var moduleInfo in moduleSlotInfos)
        {
            Debug.Log(moduleInfo.ModuleSlot.Type + " is adorned: " + moduleInfo.IsAdorned);
        }
    }
}