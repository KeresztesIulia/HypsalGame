using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
public class script_EndGameLogger : MonoBehaviour, interface_PersistentData
{
    public static script_EndGameLogger Instance;
    static bool submitted = false;

    [Header("Email details")]
    [SerializeField] string email = "101juliakeresztes@gmail.com";
    [SerializeField] string subject = "Testing sending";

    #region formInfo
    [Header("Form links")]
    [SerializeField] string url = "https://docs.google.com/forms/d/e/1FAIpQLSflX0Yjqdwd6EYJTkfUAz7uRLVeXCp3AQHBu99M4LNPewfPTA/viewform?usp=pp_url";
    [SerializeField] string responseURL = "https://docs.google.com/forms/d/e/1FAIpQLSflX0Yjqdwd6EYJTkfUAz7uRLVeXCp3AQHBu99M4LNPewfPTA/formResponse";

    [Header("Entry numbers")]
    [SerializeField] string entry_PlayStart = "entry.835986269";
    [SerializeField] string entry_FinalLabelingOutput = "entry.1821536749";
    [SerializeField] string entry_ContinuousLabelingOutput = "entry.1309834620";
    [SerializeField] string entry_TimeSpent_readable = "entry.710532270";
    [SerializeField] string entry_TimeSpent_number = "entry.516534672";
    [SerializeField] string entry_ExplanationNumber = "entry.";
    [SerializeField] string entry_ExplanationText = "entry.";
    [SerializeField] string entry_Feedback = "entry.";
    [SerializeField] string entry_SoundsTriggered = "entry.";
    #endregion

    // CollectedData
    string data_StartTime = "";
    string data_FinalLabelingOutput = "";
    string data_ContinuousLabelingOutput = "";
    string data_GameTime_readable = "";
    int data_GameTime_number;
    string data_TriggeredSounds = "";

    // needs implementation
    static string data_YellowHallwayWhiteboard = "";
    static int data_WhiteboardNumber;
    static string data_FeedbackWhiteboard = "";

    public void QuitApplication()
    {
        if (submitted) return;
        GatherAllOutputs();
        StartCoroutine(SubmitForm());
        //SubmitFormAtOnce();
        //Mail();
        //submitted = true;

    }

    public static void QuitApplication_static()
    {
        if (Instance == null) return;

        Instance.QuitApplication();
    }

    void Mail()
    {
        email = Uri.EscapeDataString(email);
        subject = Uri.EscapeDataString(subject);

        string allData = $"[Playtest date and time]\n{data_StartTime}\n\n" +
            $"[Final Labels]\n{data_FinalLabelingOutput}\n\n" +
            $"[Continuous labels]\n{data_ContinuousLabelingOutput}\n\n" +
            $"[Time data]\nIn-game time: {data_GameTime_readable} ({data_GameTime_number}s)\n\n" +
            $"[Sounds triggered]\n{data_TriggeredSounds}\n\n" +
            $"[Labeling explanation]\nWhiteboard {data_WhiteboardNumber}\n{data_YellowHallwayWhiteboard}\n\n"+
            $"[Feedback board]\n{data_FeedbackWhiteboard}"
            ;

        allData = Uri.EscapeDataString(allData);

        Application.OpenURL($"mailto:{email}?subject={subject}&body={allData}");
        //Application.OpenURL(“mailto:” +email + “?subject =” +subject + “&body =” +body + “&attachment =” +attachment);
        Close();
    }

    void SubmitFormAtOnce()
    {
        string fullUrl = url
            //+ $"&{entry_PlayStart}={UnityWebRequest.EscapeURL(data_StartTime)}"
            + $"&{entry_FinalLabelingOutput}={Uri.EscapeDataString("data_FinalLabelingOutput")}"
            //+ $"&{entry_ContinuousLabelingOutput}={UnityWebRequest.EscapeURL(data_ContinuousLabelingOutput)}"
            //+ $"&{entry_TimeSpent_readable}={UnityWebRequest.EscapeURL(data_GameTime_readable)}"
            //+ $"&{entry_TimeSpent_number}={UnityWebRequest.EscapeURL(data_GameTime_number.ToString("F0"))}"
            ;

        // Open in browser
        Application.OpenURL(fullUrl);
        Close();
    }

    IEnumerator SubmitForm()
    {
        Debug.Log("Submitting form...");

        WWWForm form = new WWWForm();

        // Add fields
        form.AddField(entry_PlayStart, data_StartTime);
        form.AddField(entry_FinalLabelingOutput, data_FinalLabelingOutput);
        form.AddField(entry_ContinuousLabelingOutput, data_ContinuousLabelingOutput);
        form.AddField(entry_TimeSpent_readable, data_GameTime_readable);
        form.AddField(entry_TimeSpent_number, data_GameTime_number);
        form.AddField(entry_ExplanationNumber, data_WhiteboardNumber);
        form.AddField(entry_ExplanationText, data_YellowHallwayWhiteboard);
        form.AddField(entry_Feedback, data_FeedbackWhiteboard);
        form.AddField(entry_SoundsTriggered, data_TriggeredSounds);

        UnityWebRequest request = UnityWebRequest.Post(responseURL, form);

        yield return request.SendWebRequest();
        //Debug.Log(request.url);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Form submission failed: " + request.error);

        }
        else
        {
            Debug.Log("Form submitted successfully!");
        }

        submitted = true;

        //Close();
    }

    public void Close()
    {

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
    }

    void GatherAllOutputs()
    {
        GetFinalLogOutput();
        GatherTimeData();
    }

    void GetFinalLogOutput()
    {
        if (!script_LabelAssociationHandler.InstanceExists) return;

        foreach (var keyValue in script_LabelAssociationHandler.Instance.Associations)
        {
            string row;

            string associatedLabel = keyValue.Value.associatedLabel?.InternalName;
            var representingModel = keyValue.Value.representingModel;
            string modelName = representingModel == null ? "No model" : representingModel.name;

            row = $"{keyValue.Key} = {associatedLabel} ({modelName})";

            if (string.IsNullOrEmpty(data_FinalLabelingOutput)) data_FinalLabelingOutput = row;
            else data_FinalLabelingOutput = string.Join("\n", data_FinalLabelingOutput, row);
        }
    }

    void AddContinuousLabelingData(Label label1, Label label2, GameObject model)
    {
        string newRow = $"{label1.InternalName} = {label2.InternalName} ({(model == null ? "No model" : model.name)})";
        if (string.IsNullOrEmpty(data_ContinuousLabelingOutput))
        {
            data_ContinuousLabelingOutput = newRow;
        }
        else data_ContinuousLabelingOutput = string.Join("\n", data_ContinuousLabelingOutput, newRow);

    }

    void GatherTimeData()
    {
        var timeSpentInGame = Time.realtimeSinceStartup;
        // Total ingame time
        int hoursSpent = (int)Mathf.Floor(timeSpentInGame / 3600);
        int minutesSpent = (int)Mathf.Floor(timeSpentInGame / 60) - hoursSpent * 60;
        int secondsSpent = (int)Mathf.Floor(timeSpentInGame) - minutesSpent * 60 - hoursSpent * 3600;
        data_GameTime_readable = $"{(hoursSpent > 0 ? (hoursSpent + "h ") : "")} {(minutesSpent > 0 ? (minutesSpent + "m ") : "")} {(secondsSpent > 0 ? (secondsSpent + "s ") : "")}";
        data_GameTime_number = (int)timeSpentInGame;
    }

    #region public helpers
    public void AddTriggeredSound(string soundName)
    {
        if (string.IsNullOrEmpty(data_TriggeredSounds))
        {
            data_TriggeredSounds = soundName;
        }
        else
        {
            data_TriggeredSounds = string.Join("\n", data_TriggeredSounds, soundName);
        }
    }

    public void AddWhiteboardText(string explanation)
    {
        data_YellowHallwayWhiteboard = explanation;
    }

    public void AddWhiteboardNumber(int number)
    {
        data_WhiteboardNumber = number;
    }

    public void AddFeedbackText(string feedback)
    {
        data_FeedbackWhiteboard = feedback;
    }
    #endregion

    #region initialization
    
    bool initialized = false;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        Instance = this;
        data_StartTime = DateTime.Now.ToLocalTime().ToString();

        if (script_LabelAssociationHandler.InstanceExists)
            script_LabelAssociationHandler.Instance.Associated += AddContinuousLabelingData;

        initialized = true;
    }

    private void OnApplicationQuit()
    {
        QuitApplication();
    }
    #endregion
}