using UnityEngine;

public class script_EndGameLogger : MonoBehaviour, interface_PersistentData
{
    // Labelings
    string continuousLabelingOutput = "[Continuous Labeling Output]";

    // Un-labelings? maybee?

    // Whiteboard data -- but how to connect to that... hmmm

    // Other parameters
    float timeSpentInGame = 0;

    private void OnApplicationQuit()
    {
        Debug.Log(GatherOutputs());
    }

    string GatherOutputs()
    {
        return string.Join("\n\n",
            GetFinalLogOutput(),
            continuousLabelingOutput,
            OtherOutput()
        );
    }

    string GetFinalLogOutput()
    {
        if (!script_LabelAssociationHandler.InstanceExists) return "";

        string output = "[Final Label Output]";

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

    string OtherOutput()
    {
        string otherOutputData = "[Other data]";
        // Total ingame time
        int hoursSpent = (int)Mathf.Floor(timeSpentInGame / 3600);
        int minutesSpent = (int)Mathf.Floor(timeSpentInGame / 60) - hoursSpent * 60;
        int secondsSpent = (int)Mathf.Floor(timeSpentInGame) - minutesSpent * 60 - hoursSpent * 3600;
        string timeSpentString = $"{(hoursSpent > 0 ? (hoursSpent + "h ") : "")} {(minutesSpent > 0 ? (minutesSpent + "m ") : "")} {(secondsSpent > 0 ? (secondsSpent + "s ") : "")}";
        otherOutputData = string.Join("\n", otherOutputData, $"In-game time: {timeSpentString} ({timeSpentInGame} seconds)");

        //

        return otherOutputData;
    }

    private void Update()
    {
        timeSpentInGame += Time.unscaledDeltaTime;
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
