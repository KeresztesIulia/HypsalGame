using UnityEngine;

public class script_PersistentInitializer : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject[] _subModules;
    [SerializeField] bool _instantiateSubmodules = false;
    [SerializeField] bool _parentSubmodulesToSelf = true;

    bool initialized = false;
    public void Initialize()
    {
        if (initialized) return;
        foreach (var module in _subModules)
        {
            var finalModule = module;
            if (_instantiateSubmodules) finalModule = Instantiate(finalModule);
            if (_parentSubmodulesToSelf) finalModule.transform.SetParent(transform);
            foreach (var persistentData in finalModule.GetComponents<MonoBehaviour>())
            {
                if (!persistentData.enabled) continue;
                if (persistentData is interface_PersistentData)
                    (persistentData as interface_PersistentData).Initialize();
            }
        }
        
        initialized = true;
    }

    private void Awake()
    {
        if (!initialized)
            Initialize();
    }
}
