using UnityEngine.InputSystem;
using UnityEngine;
using System;

public static class static_LabelingHandler
{
    static script_PlayerInteraction playerInteraction;
    static Action<InputAction.CallbackContext>[] actions;

    public static void ActivateFunctionality(interface_Interactable target)
    {
        playerInteraction = GameObject.FindFirstObjectByType<script_PlayerInteraction>();

        var representative = target as script_LabelRepresentative;
        var promptText = representative.Prompt;

        SetupChoices(representative.RepresentedLabel, representative.FilteredAssociations(), promptText);
    }

    static void SetupChoices(Label representedLabel, Label[] possibleLabels, string promptText)
    {
        var choiceCount = possibleLabels.Length;

        actions = new Action<InputAction.CallbackContext>[choiceCount];
        for (int i = 0; i < choiceCount; i++)
        {
            var label = possibleLabels[i];
            actions[i] = (ctx) => Choose(representedLabel, label, promptText);
            script_InputManager.AssignNumberAction(i + 1, actions[i]);
        }

    }

    static void Choose(Label representedLabel, Label assignedLabel, string promptText)
    {
        representedLabel.SetLabel(assignedLabel);

        script_ui_LabelLog.LogLabelingExchange(promptText, assignedLabel);

        playerInteraction.ResetTarget();
    }

    public static void DisableFunctionality()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            script_InputManager.UnassignNumberAction(i + 1, actions[i]);
        }
    }


}
