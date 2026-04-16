using TMPro;
using UnityEngine;

public class script_ui_LabelingPrompt : MonoBehaviour
{
    [SerializeField] TMP_Text _prompt;
    [SerializeField] TMP_InputField _inputField;

    LabelableWord representedWord;

    public void Initialize(string promptText, LabelableWord wordToRepresent)
    {
        _prompt.text = promptText;
        InitializeInput(wordToRepresent);
        gameObject.SetActive(true);
        script_InputManager.playerInput.enabled = false;
        script_ui_InteractionUI.DisableUI();
        script_ui_DotController.DisableDot();


        //script_InputManager.SwitchInputMap(script_InputManager.map_uiMap);
    }

    void InitializeInput(LabelableWord wordToRepresent)
    {
        representedWord = wordToRepresent;
        _inputField.onSubmit.AddListener((eventData) =>
        {
            OnSubmit();
        });

    }

    void OnSubmit()
    {
        if (ValidLabel())
        {
            representedWord.LabelWord(_inputField.text);
            script_InputManager.playerInput.enabled = true;

            script_ui_LabelLog.LogLabelingExchange(_prompt.text, _inputField.text);

            //script_InputManager.SwitchInputMap(script_InputManager.map_PlayerMap);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(script_InputManager.playerInput.currentActionMap);
    }

    bool ValidLabel()
    {
        return !string.IsNullOrEmpty(_inputField.text);
    }
}
