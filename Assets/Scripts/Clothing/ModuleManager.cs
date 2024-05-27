/*
 * File: ModuleManager.cs
 * Purpose: Manage the module pool
 * Author: Johnathan
 * Contributions: Assisted by GitHub Copilot
 */

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This class manages the module pool
/// It creates the module pool on start
/// It provides methods to get and return modules
/// </summary>
public class ModuleManager : MonoBehaviour
{
    [SerializeField] private List<ModuleData> ModuleData; // The list of module scriptable objects the pool will be created from
    public List<ModuleController> ModulePool { get; private set; } = new(); // The list of modules in the pool
    private GameObject ModulePoolParent;    // The parent object for the module pool

    #region Singleton
    public static ModuleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        // create module pool parent
        ModulePoolParent = new GameObject("ModulePool");

        // create module pool
        foreach (var data in ModuleData)
        {
            ModuleController newModule = data.CreateModule();
            ModulePool.Add(newModule);
            newModule.gameObject.SetActive(false);
            newModule.transform.SetParent(ModulePoolParent.transform);
        }
    }

    /// <summary>
    /// Get a random module of the specified type
    /// </summary>
    /// <param name="type">The type of module to get</param>
    /// <returns>The module controller</returns>
    public ModuleController GetModule(ModuleType type)
    {
        // get all modules of type
        var modules = ModulePool.Where(m => m.Type == type).ToList();

        if (modules.Count > 0)
        {
            // choose random module from list
            var module = modules[Random.Range(0, modules.Count)];
            module.gameObject.SetActive(true);
            // remove module from pool and return
            ModulePool.Remove(module);
            return module;
        }

        return null;
    }

    /// <summary>
    /// Return a module to the pool
    /// </summary>
    public void ReturnModule(ModuleController module)
    {
        module.gameObject.SetActive(false);
        // add module back to pool
        ModulePool.Add(module);
        module.transform.SetParent(ModulePoolParent.transform);
    }

    /// <summary>
    /// Log the modules in the pool
    /// </summary>
    private void LogModules(List<ModuleController> modules)
    {
        string logMessage = "";
        foreach (var module in modules)
        {
            logMessage += $"{module.name} {module.Type}, ";
        }
    }
}