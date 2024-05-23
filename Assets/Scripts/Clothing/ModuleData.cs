using UnityEngine;

[CreateAssetMenu(fileName = "ModuleData")]
public class ModuleData : ScriptableObject
{
    public ModuleType type;
    public string style;
    public GameObject prefab;

    public ModuleController CreateModule()
    {
        GameObject module = Instantiate(prefab);
        module.AddComponent<ModuleController>().Init(this);
        return module.GetComponent<ModuleController>();
    }
}
