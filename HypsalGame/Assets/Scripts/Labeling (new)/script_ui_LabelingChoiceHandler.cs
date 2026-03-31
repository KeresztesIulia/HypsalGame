using UnityEngine;

public class script_ui_LabelingChoiceHandler : MonoBehaviour, interface_PersistentData
{
    [SerializeField] script_ui_LabelingChoicePrompt _promptPrefab;

    public static script_ui_LabelingChoiceHandler Instance;

    public void Initialize()
    {
        Instance = this;
    }

    public static void InstantiatePrompt(script_LabelRepresentative representative)
    {
        script_ui_LabelingChoicePrompt currentPrompt = Instantiate(Instance?._promptPrefab, Instance?.transform);
        currentPrompt.Initialize(representative.Prompt, representative.RepresentedLabel, representative.PossibleLabels);
    }
}
