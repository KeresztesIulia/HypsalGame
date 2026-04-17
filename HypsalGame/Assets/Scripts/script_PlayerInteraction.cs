using UnityEngine;

public class script_PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _interactableNoticeDistance = 10f;
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
                    DisablePreviousInteractionFunctionality();
                    DisablePreviousInteractionUI();
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
                static_InteractionHandler.DisableFunctionality();
                break;
            case interface_Interactable.InteractionType.Labelable:
                static_LabelingHandler.DisableFunctionality(); 
                break;
            default:
                // Visual has no functionality, that's the point
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
                    script_ui_InteractionUI.DisableUI();
                    break;
                case interface_Interactable.InteractionType.Labelable:
                    script_ui_LabelingChoiceHandler.DisableUI(currentTarget);
                    break;
                case interface_Interactable.InteractionType.Visual:
                    script_ui_VisualHandler.DisableUI();
                    break;
                default:
                    break;
            }
        }
    }

    void ActivateCurrentInteractionUI()
    {
        switch (currentTarget.interactionType)
        {
            case interface_Interactable.InteractionType.Generic:
                script_ui_InteractionUI.ActivateUI(currentTarget);
                break;
            case interface_Interactable.InteractionType.Labelable:
                script_ui_LabelingChoiceHandler.ActivateUI(currentTarget);
                break;
            case interface_Interactable.InteractionType.Visual:
                script_ui_VisualHandler.ActivateUI(currentTarget);
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
                static_InteractionHandler.ActivateFunctionality(currentTarget, transform.position);
                break;
            case interface_Interactable.InteractionType.Labelable:
                static_LabelingHandler.ActivateFunctionality(currentTarget);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, _interactableNoticeDistance, ~_raycastIgnoreLayer))
        {
            bool inRange = hitInfo.distance < _interactionDistance;
            bool found = false;
            bool foundInRange = false;

            foreach (var potentialTarget in hitInfo.transform.GetComponentsInParent<interface_Interactable>(true))
            {
                if ((potentialTarget as MonoBehaviour).isActiveAndEnabled)
                {
                    found = true;
                    if (!inRange) break;

                    CurrentTarget = potentialTarget;
                    foundInRange = true;
                    break;
                    
                }
            }

            script_ui_DotController.SetInteractableDotState(found);

            if (!foundInRange)
            {
                CurrentTarget = null;
            }
        }
        else
        {
            CurrentTarget = null;
            // Activate normal dot / deactivate extra circle
            script_ui_DotController.SetInteractableDotState(false);
        }

    }

    public void ResetTarget()
    {
        CurrentTarget = null;

    }
    
}
