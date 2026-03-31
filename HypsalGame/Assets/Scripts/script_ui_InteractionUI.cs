using TMPro;
using UnityEngine;

public class script_ui_InteractionUI : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject _interactionInfoHolder;
    [SerializeField] TMP_Text _objectNameText;
    [SerializeField] TMP_Text _interactionWordText;
    [SerializeField] GameObject _dot;

    script_Interactable currentTarget = null;

    public static script_ui_InteractionUI Instance;

    bool initialized = false;

    // funcHardcode! move later!!!
    bool functionalityInitialized = false;

    public void Initialize()
    {
        Debug.Log("Initialized interaction UI");
        Instance = this;
        DisableUI();
        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
            Initialize();
    }


    public static void DisableUI(bool disableDot = false)
    {
        Instance?._interactionInfoHolder.SetActive(false);
        Instance?._dot.SetActive(!disableDot);

        Debug.Log("Disabled interaction UI");
    }

    public static void ActivateUI(UIInfo uiInfo)
    {
        if (Instance == null)
        {
            Debug.LogError("No InteractionUI present");
            return;
        }

        Debug.Log("Updating interaction UI with new target");

        Instance._dot.SetActive(true);

        Instance._objectNameText.text = uiInfo.name;
        Instance._interactionWordText.text = uiInfo.interactionWord;

        Instance._interactionInfoHolder.SetActive(true);
    }

    public static void ActivateUI(interface_Interactable interactable)
    {
        ActivateUI((interactable as script_Interactable)._uiInfo);
    }

    [System.Serializable]
    public struct UIInfo
    {
        public string name;
        public string interactionWord;
    }


    // funcHardcode! move later!!!
    public void ActivateFunctionality(interface_Interactable target, Vector3 playerPosition)
    {
        currentTarget = target as script_Interactable;
        if (functionalityInitialized) return;
        script_InputManager.action_Interact.performed += (ctx) => OnInteraction(playerPosition);
        functionalityInitialized = true;
    }

    void OnInteraction(Vector3 playerPosition)
    {
        if (currentTarget == null) return;
        currentTarget.Interact(playerPosition);
    }

    public void DisableFunctionality()
    {
        currentTarget = null;
    }
}
