using UnityEngine;

public class script_PlayerInteraction : MonoBehaviour
{
    //Camera playerCamera;
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
