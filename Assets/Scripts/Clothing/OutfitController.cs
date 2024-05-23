using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ModuleType { Head, Torso, Legs }

[Serializable]
public struct ModuleParent
{
    [field: SerializeField] public ModuleType Type { get; private set; }
    [field: SerializeField] public GameObject Parent { get; private set; }
    [field: SerializeField] public GameObject BaseModule { get; private set; }
}

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


public class OutfitController : MonoBehaviour
{
    [SerializeField] private List<ModuleParent> moduleParents = new();
    private List<ModuleInfo> moduleInfos = new();
    private ModuleManager moduleManager;

    private List<ModuleInfo> CurrentOutfit => moduleInfos.Where(info => info.IsAdorned).ToList();
    public bool IsNude => CurrentOutfit.Count == 0;

    private ModuleInfo GetModuleInfo(ModuleType type) => moduleInfos.First(info => info.ModuleParent.Type == type);

    void Start()
    {
        Debug.Log("OutfitController started");
        moduleManager = ModuleManager.Instance;
        foreach (var moduleParent in moduleParents)
        {
            moduleInfos.Add(new ModuleInfo(moduleParent));
        }
    }

    public void SetModule(ModuleType type)
    {
        var module = moduleManager.GetModule(type);
        var moduleInfo = GetModuleInfo(type);
        var moduleParent = moduleInfo.ModuleParent;
        // if (moduleInfo.IsAdorned)
        // {
        //     moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
        // }
        moduleParent.BaseModule.SetActive(false);
        moduleInfo.CurrentAdornment = module;
        Debug.Log("Module " + moduleParent.Type + " is adorned: " + moduleInfo.IsAdorned);
        module.transform.SetParent(moduleParent.Parent.transform);
        module.transform.localPosition = Vector3.zero;
        // set y rotation to match the base module
        module.transform.localRotation = moduleParent.BaseModule.transform.localRotation;
    }

    public void RemoveModule()
    {
        if (IsNude) { return; }
        var moduleInfo = CurrentOutfit[UnityEngine.Random.Range(0, CurrentOutfit.Count)];
        moduleManager.ReturnModule(moduleInfo.CurrentAdornment);
        moduleInfo.CurrentAdornment = null;
        moduleInfo.ModuleParent.BaseModule.SetActive(true);
    }

    void OnDestroy()
    {
        Debug.Log("Destroying OutfitController");
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
        }
    }
}