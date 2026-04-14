using UnityEngine;

public static class static_InteractionHandler
{
    static script_Interactable currentTarget;
    static bool functionalityInitialized = false;

    public static void ActivateFunctionality(interface_Interactable target, Vector3 playerPosition)
    {
        currentTarget = target as script_Interactable;
        if (functionalityInitialized) return;
        script_InputManager.action_Interact.performed += (ctx) => OnInteraction(playerPosition);
        functionalityInitialized = true;
    }

    static void OnInteraction(Vector3 playerPosition)
    {
        if (currentTarget == null) return;
        currentTarget.Interact(playerPosition);
    }

    public static void DisableFunctionality()
    {
        currentTarget = null;
    }
}
