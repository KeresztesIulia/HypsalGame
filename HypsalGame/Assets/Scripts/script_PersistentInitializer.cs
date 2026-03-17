using UnityEngine;

public class script_PersistentInitializer : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject[] _subModules;
    public void Initialize()
    {
        foreach (var module in _subModules)
            foreach (var persistentData in module.GetComponents<Component>())
                if (persistentData is interface_PersistentData)
                    (persistentData as interface_PersistentData).Initialize();
    }
}
