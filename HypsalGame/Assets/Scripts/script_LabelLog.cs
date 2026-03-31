using TMPro;
using UnityEngine;

public class script_LabelLog : MonoBehaviour, interface_PersistentData
{
    [Header("Text prefabs")]
    [SerializeField] TMP_Text _aiTextPrefab;
    [SerializeField] TMP_Text _aiThankYouPrefab;
    [SerializeField] TMP_Text _playerTextPrefab;

    [Header("Log elements")]
    [SerializeField] GameObject _container;
    [SerializeField] Transform _contentTransform;

    public enum LogType { AI, ThankYou, Player };
    public static script_LabelLog Instance;

    bool initialized = false;

    public void Initialize()
    {
        Instance = this;
        script_InputManager.action_ShowLog.performed += (ctx) => ToggleLog();
        initialized = true;
    }

    void Start()
    {
        if (!initialized) Initialize();
    }

    void ToggleLog()
    {
        _container.SetActive(!_container.activeSelf);
    }

    public static void LogText(LogType logType, string logText = "")
    {
        switch (logType)
        {
            case LogType.AI:
                LogAIText(logText); break;
            case LogType.ThankYou:
                LogAIThankYou(); break;
            case LogType.Player:
                break;
        }
    }

    public static void LogText(TMP_Text textBox, string logText)
    {
        textBox.text = logText;
        textBox.transform.SetParent(Instance?._contentTransform, false);
    }

    public static void LogAIText(string logText)
    {
        TMP_Text aiText = Instantiate(Instance?._aiTextPrefab);
        LogText(aiText, logText);
    }

    public static void LogAIThankYou()
    {
        TMP_Text thankYouText = Instantiate(Instance?._aiThankYouPrefab, Instance?._contentTransform);
    }

    public static void LogPlayerText(string logText)
    {
        TMP_Text aiText = Instantiate(Instance?._playerTextPrefab);
        LogText(aiText, logText);
    }

    public static void LogLabelingExchange(string labelingPrompt, string playerChoice)
    {
        LogAIText(labelingPrompt);
        LogPlayerText(playerChoice);
        LogAIThankYou();
    }
}
