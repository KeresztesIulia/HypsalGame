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
        SetupChoices(representative);
    }

    static void SetupChoices(script_LabelRepresentative representative)
    {
        var possibleLabels = representative.FilteredAssociations();
        var choiceCount = possibleLabels.Length;

        actions = new Action<InputAction.CallbackContext>[choiceCount];
        for (int i = 0; i < choiceCount; i++)
        {
            var label = possibleLabels[i];
            actions[i] = (ctx) => Choose(representative, label);
            script_InputManager.AssignNumberAction(i + 1, actions[i]);
        }
    }

    static void Choose(script_LabelRepresentative representative, Label assignedLabel)
    {
        var promptText = representative.Prompt;
        representative.RepresentedLabel.SetLabel(assignedLabel);

        string specialResponse = script_LabelingResponseHandler.GetResponse(representative.RepresentedLabel, assignedLabel);

        script_ui_LabelLog.LogLabelingExchange(promptText, assignedLabel.OriginalDisplayName, specialResponse);

        playerInteraction.ResetTarget();
        representative.LabeledObject?.Invoke();
    }

    public static void DisableFunctionality()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            script_InputManager.UnassignNumberAction(i + 1, actions[i]);
        }
    }


}
