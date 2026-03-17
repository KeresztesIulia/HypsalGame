using UnityEngine;

public abstract class script_Interactable : MonoBehaviour
{
    public script_ui_InteractionUI.UIInfo _uiInfo;

    public abstract void Interact(Vector3 playerPosition);

    
}
