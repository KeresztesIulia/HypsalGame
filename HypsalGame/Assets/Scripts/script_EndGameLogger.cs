using UnityEngine;

public class script_EndGameLogger : MonoBehaviour, interface_PersistentData
{
    // Labelings
    string continuousLabelingOutput = "[Continuous Labeling Output]";

    // Un-labelings? maybee?

    private void OnApplicationQuit()
    {
        Debug.Log(GetFinalLogOutput());
        Debug.Log(continuousLabelingOutput);
    }

    string GetFinalLogOutput()
    {
        if (!script_LabelAssociationHandler.InstanceExists) return "";

        string output = "[Final Log Output]";

        foreach (var keyValue in script_LabelAssociationHandler.Instance.Associations)
        {
            string row;
            
            string associatedLabel = keyValue.Value.associatedLabel?.InternalName;
            var representingModel = keyValue.Value.representingModel;
            string modelName = representingModel == null ? "No model" : representingModel.name;

            row = $"{keyValue.Key} = {associatedLabel} ({modelName})";
            output = string.Join("\n", output, row);
        }

        return output;
    }

    // ----------- INITIALIZATION -------------

    bool initialized = false;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized || !script_LabelAssociationHandler.InstanceExists) return;

        script_LabelAssociationHandler.Instance.Associated += (label1, label2, model) =>
            continuousLabelingOutput = string.Join("\n", continuousLabelingOutput,
            $"{label1.InternalName} = {label2.InternalName} ({(model == null ? "No model" : model.name)})");

        initialized = true;
    }
}
