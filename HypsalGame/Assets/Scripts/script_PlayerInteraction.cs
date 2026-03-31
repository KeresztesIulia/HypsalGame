using UnityEngine;

public class script_PlayerInteraction : MonoBehaviour
{
    // redo should focus on more generalized "interactables", as in looking at something is also interaction
    // we have, looking at something, generic interaction (what is implemented here atm), and labeling (which should not go through the interaction keypress)
    // so for these 3 categories, have different (maybe not even MonoBehaviour) classes, and the main interaction script delegates the specifics to each class
    // the point is to identify in one place what we're dealing with

    [SerializeField] float _interactionDistance = 5f;
    [SerializeField] LayerMask _raycastIgnoreLayer;

    interface_Interactable currentTarget = null;
    interface_Interactable.InteractionType previousInteractionType = interface_Interactable.InteractionType.None;
    public interface_Interactable CurrentTarget
    {
        get
        {
            return currentTarget;
        }
        private set
        {
            if (value != currentTarget)
            {
                currentTarget = value;
                if (value == null)
                {
                    script_ui_InteractionUI.DisableUI();
                    script_ui_LabelingChoiceHandler.DestroyCurrentPrompt();
                    previousInteractionType = interface_Interactable.InteractionType.None;
                }
                else
                {
                    DisablePreviousInteractionFunctionality();
                    DisablePreviousInteractionUI();
                    ActivateCurrentInteractionUI();
                    ActivateCurrentInteractionFunctionality();
                    previousInteractionType = currentTarget.interactionType;
                }
            }
        }
    }

    void DisablePreviousInteractionFunctionality()
    {
        var currentInteractionType = currentTarget?.interactionType;
        switch (previousInteractionType)
        {
            case interface_Interactable.InteractionType.Generic:
                script_ui_InteractionUI.DisableUI(true);
                break;
            case interface_Interactable.InteractionType.Labelable:
                script_ui_LabelingChoiceHandler.DestroyCurrentPrompt();
                break;
            default:
                break;
        }
    }

    void DisablePreviousInteractionUI()
    {
        var currentInteractionType = currentTarget?.interactionType;
        if (currentInteractionType != previousInteractionType)
        {
            switch (previousInteractionType)
            {
                case interface_Interactable.InteractionType.Generic:
                    script_ui_InteractionUI.DisableUI(true);
                    break;
                case interface_Interactable.InteractionType.Labelable:
                    script_ui_LabelingChoiceHandler.DisableUI(currentTarget);
                    break;
                default:
                    break;
            }
        }
    }

    void ActivateCurrentInteractionUI()
    {
        // for now, we give UI and functionality all in one, because I see no better way at the moment
        switch (currentTarget.interactionType)
        {
            case interface_Interactable.InteractionType.Generic:
                script_ui_InteractionUI.ActivateUI(currentTarget);
                break;
            case interface_Interactable.InteractionType.Labelable:
                script_ui_LabelingChoiceHandler.ActivateUI(currentTarget);
                break;
            default:
                break;
        }
    }

    void ActivateCurrentInteractionFunctionality()
    {
        switch (currentTarget.interactionType)
        {
            case interface_Interactable.InteractionType.Generic:
                // give target to script_Interactable
                script_ui_InteractionUI.Instance?.ActivateFunctionality(currentTarget, transform.position);
                break;
            case interface_Interactable.InteractionType.Labelable:
                // give target to script_LabelingHandle
                // for now, functionality stays with the UI, separate later!
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, _interactionDistance, ~_raycastIgnoreLayer))
        {
            CurrentTarget = hitInfo.transform.GetComponent<interface_Interactable>();
        }
        else
        {
            CurrentTarget = null;
        }

    }
}
