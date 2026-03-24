using TMPro;
using UnityEngine;

public class script_ui_InteractionUI : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject _interactionInfoHolder;
    [SerializeField] TMP_Text _objectNameText;
    [SerializeField] TMP_Text _interactionWordText;
    [SerializeField] GameObject _dot;

    static script_ui_InteractionUI instance;

    bool initialized = false;

    public void Initialize()
    {
        instance = this;
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
        instance?._interactionInfoHolder.SetActive(false);
        instance?._dot.SetActive(!disableDot);

        Debug.Log("Disabled interaction UI");
    }

    public static void ActivateUI(UIInfo uiInfo)
    {
        if (instance == null)
        {
            Debug.LogError("No InteractionUI present");
            return;
        }

        Debug.Log("Updating interaction UI with new target");

        instance._dot.SetActive(true);

        instance._objectNameText.text = uiInfo.name;
        instance._interactionWordText.text = uiInfo.interactionWord;

        instance._interactionInfoHolder.SetActive(true);
    }

    [System.Serializable]
    public struct UIInfo
    {
        public string name;
        public string interactionWord;
    }
}
