using UnityEngine;

public class script_ui_LabelingChoiceHandler : MonoBehaviour, interface_PersistentData
{
    [SerializeField] script_ui_LabelingChoicePrompt _promptPrefab;

    public static script_ui_LabelingChoiceHandler Instance;

    script_ui_LabelingChoicePrompt currentPrompt;

    public void Initialize()
    {
        Instance = this;
    }

    public static void InstantiatePrompt(script_LabelRepresentative representative)
    {
        if (Instance?.currentPrompt != null) Destroy(Instance.currentPrompt.gameObject);
        Instance.currentPrompt = Instantiate(Instance?._promptPrefab, Instance?.transform);
        Instance.currentPrompt.Initialize(representative.Prompt, representative.RepresentedLabel, representative.PossibleLabels);
    }
}
