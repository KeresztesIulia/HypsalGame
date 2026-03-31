using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class script_ui_LabelingChoicePrompt : MonoBehaviour
{
    [SerializeField] TMP_Text promptText;
    [SerializeField] Transform choiceContainer;

    [SerializeField] script_ui_ChoicePrompt choicePrefab;

    Label representedLabel;

    public void Initialize(string prompt, Label representedLabel, List<Label> choiceLabels)
    {
        promptText.text = prompt;
        this.representedLabel = representedLabel;

        SetupChoices(choiceLabels);

        gameObject.SetActive(true);
        script_InputManager.playerInput.enabled = false;
        script_ui_InteractionUI.DisableUI(true);

    }   
    
    void SetupChoices(List<Label> choiceLabels)
    {
        // shuffle later
        for (int i = 0; i < choiceLabels.Count; i++)
        {
            var choicePrompt = Instantiate(choicePrefab, choiceContainer);
            //script_InputManager.action_Number[i]
            choicePrompt.Initialize(i + 1, choiceLabels[i], Choose);
        }
    }

    void Choose(int index, Label label)
    {
        representedLabel.SetLabel(label);
        script_InputManager.playerInput.enabled = true;
        script_LabelLog.LogLabelingExchange(promptText.text, label);

        Destroy(gameObject);
    }

}
