using UnityEngine;

public class script_PlayerInteraction : MonoBehaviour
{
    // redo should focus on more generalized "interactables", as in looking at something is also interaction
    // we have, looking at something, generic interaction (what is implemented here atm), and labeling (which should not go through the interaction keypress)
    // so for these 3 categories, have different (maybe not even MonoBehaviour) classes, and the main interaction script delegates the specifics to each class
    // the point is to identify in one place what we're dealing with

    [SerializeField] float _interactionDistance = 5f;
    [SerializeField] LayerMask _raycastIgnoreLayer;

    script_Interactable currentTarget = null;
    public script_Interactable CurrentTarget
    {
        get
        {
            return currentTarget;
        }
        private set
        {
            if (value != currentTarget)
            {
                if (value == null)
                {
                    script_ui_InteractionUI.DisableUI();
                }
                else
                {
                    script_ui_InteractionUI.ActivateUI(value._uiInfo);
                }
            }
            currentTarget = value;
        }
    }

    private void Start()
    {
        script_InputManager.action_Interact.performed += (ctx) => OnInteraction();
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, _interactionDistance, ~_raycastIgnoreLayer))
        {
            CurrentTarget = hitInfo.transform.GetComponent<script_Interactable>();
        }
        else
        {
            CurrentTarget = null;
        }

    }

    void OnInteraction()
    {
        if (currentTarget == null) return;
            currentTarget.Interact(transform.position);
    }

    //private void OnDestroy()
    //{
    //    script_InputManager.action_Interact.performed -= (ctx) => OnInteraction();
    //}
}
