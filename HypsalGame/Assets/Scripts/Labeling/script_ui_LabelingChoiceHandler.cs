using UnityEngine;

public class script_ui_LabelingChoiceHandler : MonoBehaviour, interface_PersistentData
{
    [SerializeField] script_ui_LabelingChoicePrompt _promptPrefab;

    public static script_ui_LabelingChoiceHandler Instance;

    script_ui_LabelingChoicePrompt currentPrompt;

    bool initialized = false;

    public void Start()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        Instance = this;
        initialized = true;
    }

    public static void InstantiatePrompt(interface_Interactable representative)
    {
        InstantiatePrompt(representative as script_LabelRepresentative);
    }

    public static void ActivateUI(interface_Interactable target)
    {
        var representative = target as script_LabelRepresentative;
        if (representative.labelable)
        {
            InstantiatePrompt(representative);
        }
    }

    public static void InstantiatePrompt(script_LabelRepresentative representative)
    {
        DestroyCurrentPrompt();

        Instance.currentPrompt = Instantiate(Instance?._promptPrefab, Instance?.transform);
        Instance.currentPrompt.Initialize(representative.Prompt, representative.RepresentedLabel, representative.FilteredAssociations());
    }

    public static void DisableUI(interface_Interactable target)
    {
        DestroyCurrentPrompt();
    }

    public static void DestroyCurrentPrompt()
    {
        if (Instance?.currentPrompt != null) Instance?.currentPrompt.Dispose();

    }
}
