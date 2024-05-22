using UnityEngine;


public class ModuleController : MonoBehaviour
{
    public ModuleType type;
    public string style;
    public GameObject prefab;

    public void Init(ModuleData data)
    {
        type = data.type;
        style = data.style;
        prefab = data.prefab;
    }
}