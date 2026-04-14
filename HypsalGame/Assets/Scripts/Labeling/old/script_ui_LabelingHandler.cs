using UnityEngine;

public class script_ui_LabelingHandler : MonoBehaviour, interface_PersistentData
{
    [SerializeField] script_ui_LabelingPrompt _promptPrefab;

    public static script_ui_LabelingHandler Instance;

    bool initialized = false;

    private void Start()
    {
        if (!initialized) Initialize();
    }

    public void Initialize()
    {
        Instance = this;
        initialized = true;
    }

    public static void InstantiatePrompt(string prompt, LabelableWord wordToLabel)
    {

        script_ui_LabelingPrompt currentPrompt = Instantiate(Instance?._promptPrefab, Instance?.transform);
        currentPrompt.Initialize(prompt, wordToLabel);
    }
}
