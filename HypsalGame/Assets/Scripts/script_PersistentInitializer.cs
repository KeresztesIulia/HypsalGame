using UnityEngine;

public class script_PersistentInitializer : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject[] _subModules;
    [SerializeField] bool _instantiateSubmodules = false;

    bool initialized = false;
    public void Initialize()
    {
        foreach (var module in _subModules)
        {
            var finalModule = module;
            if (_instantiateSubmodules) finalModule = Instantiate(finalModule);
            foreach (var persistentData in module.GetComponents<MonoBehaviour>())
            {
                if (!persistentData.enabled) continue;
                if (persistentData is interface_PersistentData)
                    (persistentData as interface_PersistentData).Initialize();
            }
        }
                
        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
            Initialize();
    }
}
