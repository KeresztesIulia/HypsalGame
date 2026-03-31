using System.Collections.Generic;
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

    public static Dictionary<int, InputAction> action_Number;


    bool initialized = false;

    public void Initialize()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerInput>();
        inputActions = playerInput?.actions;

        map_PlayerMap = inputActions?.FindActionMap("Player");
        action_Interact = map_PlayerMap?.FindAction("Interact");
        action_ShowLog = map_PlayerMap?.FindAction("Show log");

        map_uiMap = inputActions?.FindActionMap("UI");

        action_Number = new Dictionary<int, InputAction>();
        for (int i = 0; i < 10; i++)
        {
            action_Number[i] = map_uiMap?.FindAction($"Number {i}");
        }


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
