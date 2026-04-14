using UnityEngine;

public abstract class script_Interactable : MonoBehaviour, interface_Interactable
{
    public script_ui_InteractionUI.UIInfo _uiInfo;

    public interface_Interactable.InteractionType interactionType => interface_Interactable.InteractionType.Generic;

    public abstract void Interact(Vector3 playerPosition);

    
}
