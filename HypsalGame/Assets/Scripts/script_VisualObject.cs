using UnityEngine;

public class script_VisualObject : MonoBehaviour, interface_Interactable
{
    public script_ui_VisualHandler.UIInfo _uiInfo;

    public interface_Interactable.InteractionType interactionType => interface_Interactable.InteractionType.Visual;

    public void SetActiveState(bool activeState)
    {
        enabled = activeState;
    }    
}
