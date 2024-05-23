using System.Collections.Generic;
using UnityEngine;

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
        foreach (var module in ModulePool)
        {
            if (module.type == type)
            {
                module.gameObject.SetActive(true);
                Debug.Log("Module " + module.type + " is active");
                
                return module;
            }
        }
        return null;
    }   

    public void ReturnModule(ModuleController module)
    {
        module.gameObject.SetActive(false);
        module.transform.SetParent(ModulePoolParent.transform);
    }
}