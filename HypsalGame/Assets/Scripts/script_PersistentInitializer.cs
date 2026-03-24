using UnityEngine;

public class script_PersistentInitializer : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject[] _subModules;

    bool initialized = false;
    public void Initialize()
    {
        foreach (var module in _subModules)
            foreach (var persistentData in module.GetComponents<Component>())
                if (persistentData is interface_PersistentData)
                    (persistentData as interface_PersistentData).Initialize();
        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
            Initialize();
    }
}
