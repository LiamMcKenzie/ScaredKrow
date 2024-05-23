using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModuleManager : MonoBehaviour
{
    [SerializeField] private List<ModuleData> ModuleData;
    public List<ModuleController> ModulePool { get; private set; } = new();
    private GameObject ModulePoolParent;

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

        foreach (var data in ModuleData)
        {
            ModuleController newModule = data.CreateModule();
            ModulePool.Add(newModule);
            newModule.gameObject.SetActive(false);
            newModule.transform.SetParent(ModulePoolParent.transform);
        }
    }

    public ModuleController GetModule(ModuleType type)
    {
        var modules = ModulePool.Where(m => m.type == type).ToList();

        LogModules(modules);

        // choose random module from list
        if (modules.Count > 0)
        {
            var module = modules[Random.Range(0, modules.Count)];
            module.gameObject.SetActive(true);
            // remove module from pool
            ModulePool.Remove(module);
            return module;
        }

        return null;
    }

    public void ReturnModule(ModuleController module)
    {
        module.gameObject.SetActive(false);
        // add module back to pool
        ModulePool.Add(module);
        module.transform.SetParent(ModulePoolParent.transform);
    }
    private void LogModules(List<ModuleController> modules)
    {
        string logMessage = "";
        foreach (var module in modules)
        {
            logMessage += $"{module.name} {module.type}, ";
        }
    }
}