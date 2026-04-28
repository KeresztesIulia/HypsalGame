using UnityEngine;

public class script_FalseInteraction : script_Interactable
{
    public override void Interact(Vector3 playerPosition)
    {
        Debug.Log("False interaction: Doing Nothing");
    }
}
