using TMPro;
using UnityEngine;

public class script_ui_VisualHandler : MonoBehaviour
{
    [SerializeField] TMP_Text _visualizerText;

    public static script_ui_VisualHandler Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void ActivateUI(interface_Interactable target)
    {

        if (Instance == null) return;
        if (target is not script_VisualObject) return;

        Instance._visualizerText.text = (target as script_VisualObject)._uiInfo._objectName;
        Instance._visualizerText?.gameObject.SetActive(true);
    }

    public static void DisableUI()
    {
        if (Instance == null) return;
        Instance._visualizerText?.gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct UIInfo
    {
        public string _objectName;
    }
}


