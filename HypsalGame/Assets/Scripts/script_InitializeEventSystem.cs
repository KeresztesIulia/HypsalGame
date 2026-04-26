using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class script_InitializeEventSystem : MonoBehaviour, interface_PersistentData
{
    bool initialized = false;
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        GameObject.FindFirstObjectByType<PlayerInput>().uiInputModule = GetComponent<InputSystemUIInputModule>();

        initialized = true;
    }

    
}
