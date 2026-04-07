using UnityEngine;

public interface interface_Interactable
{
    InteractionType interactionType { get; }
    public enum InteractionType
    {
        None,
        Generic,
        Labelable,
        Visual // only has a name/text, no functionality
    }
}
