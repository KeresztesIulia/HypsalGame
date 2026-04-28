using TMPro;
using UnityEngine;

public class script_ui_InteractionUI : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject _interactionInfoHolder;
    [SerializeField] TMP_Text _objectNameText;
    [SerializeField] TMP_Text _interactionWordText;

    public static script_ui_InteractionUI Instance;

    bool initialized = false;

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

    public static void DisableUI()
    {
        Instance?._interactionInfoHolder.SetActive(false);

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
    
}
