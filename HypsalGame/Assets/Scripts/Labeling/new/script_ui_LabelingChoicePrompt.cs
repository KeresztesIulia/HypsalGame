using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class script_ui_LabelingChoicePrompt : MonoBehaviour
{
    [SerializeField] TMP_Text promptText;
    [SerializeField] Transform choiceContainer;

    [SerializeField] script_ui_ChoicePrompt choicePrefab;

    List<Label> labels;
    System.Action<InputAction.CallbackContext>[] actions; // apparently I need to store them...

    Label representedLabel;
    int choiceCount;

    bool disposed = false;

    public void Initialize(string prompt, Label representedLabel, List<Label> choiceLabels)
    {
        promptText.text = prompt;
        this.representedLabel = representedLabel; // ?MOVE

        labels = choiceLabels;

        SetupChoices();

        gameObject.SetActive(true);
        //script_InputManager.SwitchInputMap(script_InputManager.map_uiMap); // !MOVE
    }   
    
    void SetupChoices()
    {
        // shuffle later -- how to make that consistent between UI and 
        choiceCount = labels.Count;
        actions = new System.Action<InputAction.CallbackContext>[choiceCount];
        for (int i = 0; i < choiceCount; i++)
        {
            var choicePrompt = Instantiate(choicePrefab, choiceContainer);
            var label = labels[i];
            actions[i] = (ctx) => Choose(label);
            script_InputManager.AssignNumberAction(i + 1, actions[i]);
            choicePrompt.Initialize(i + 1, label, Choose);
        }
    }

    void Choose(Label label) //!Move
    {
        representedLabel.SetLabel(label);

        //script_InputManager.SwitchInputMap(script_InputManager.map_PlayerMap); //!MOVE

        script_LabelLog.LogLabelingExchange(promptText.text, label);

        Dispose();
    }

    void UnassignActions()
    {
        for (int i = 0; i < choiceCount; i++)
        {
            var label = labels[i];
            script_InputManager.UnassignNumberAction(i + 1, actions[i]);

        }
    }

    void Dispose()
    {
        UnassignActions(); 
        Destroy(gameObject);

        disposed = true;
    }

    private void OnDestroy()
    {
        if (!disposed)
        {
            UnassignActions();
            disposed = true;
        }

    }

}
