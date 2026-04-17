using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class script_InputManager : MonoBehaviour, interface_PersistentData
{
    public static script_InputManager Instance;

    public static PlayerInput playerInput;
    public static InputActionAsset inputActions;

    public static InputActionMap map_PlayerMap;
    public static InputAction action_Interact;
    public static InputAction action_ShowLog;
    public static InputAction action_PlayerScroll;

    public static InputActionMap map_uiMap;

    public static InputAction[] action_Number;


    bool initialized = false;

    public void Initialize()
    {
        Instance = this;
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerInput>();
        inputActions = playerInput?.actions;

        map_PlayerMap = inputActions?.FindActionMap("Player");
        action_Interact = map_PlayerMap?.FindAction("Interact");
        action_ShowLog = map_PlayerMap?.FindAction("Show log");
        action_PlayerScroll = map_PlayerMap?.FindAction("ScrollWheel");

        map_uiMap = inputActions?.FindActionMap("UI");

        action_Number = new InputAction[10];
        for (int i = 0; i < 10; i++)
        {
            int idx = i;
            action_Number[i] = map_PlayerMap?.FindAction($"Number {i}");
            //action_Number[i].performed += (ctx) => Debug.Log($"perfomed {idx}");
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

    public static void AssignNumberAction(int number, System.Action<InputAction.CallbackContext> actionToPerform)
    {
        number = number % 10;
        action_Number[number].performed += actionToPerform;
    }

    public static void UnassignNumberAction(int number, System.Action<InputAction.CallbackContext> actionToPerform)
    {
        number = number % 10;
        action_Number[number].performed -= actionToPerform;
    }


}
