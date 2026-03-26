using UnityEngine;
using UnityEngine.InputSystem;

public class script_InputManager : MonoBehaviour, interface_PersistentData
{
    public static PlayerInput playerInput;
    public static InputActionAsset inputActions;

    public static InputActionMap map_PlayerMap;
    public static InputAction action_Interact;
    public static InputAction action_ShowLog;

    public static InputActionMap map_uiMap;

    bool initialized = false;

    public void Initialize()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerInput>();
        inputActions = playerInput?.actions;

        map_PlayerMap = inputActions?.FindActionMap("Player");
        action_Interact = map_PlayerMap?.FindAction("Interact");
        action_ShowLog = map_PlayerMap?.FindAction("Show log");

        map_uiMap = inputActions?.FindActionMap("UI");

        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
            Initialize();
    }

    public static void SwitchInputMap(InputActionMap to)
    {
        playerInput.currentActionMap = to;
    }
}
