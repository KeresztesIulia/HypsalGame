using UnityEngine;

public class quicktest_SendLogData : script_Interactable
{
    public override void Interact(Vector3 playerPosition)
    {
        script_EndGameLogger.Instance?.Close();
    }
}
