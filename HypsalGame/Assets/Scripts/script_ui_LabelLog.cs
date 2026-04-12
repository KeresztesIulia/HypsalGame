using TMPro;
using UnityEngine;

public class script_ui_LabelLog : MonoBehaviour, interface_PersistentData
{
    [Header("Text prefabs")] // for formatting only
    [SerializeField] TMP_Text _aiTextPrefab;
    [SerializeField] TMP_Text _playerTextPrefab;
    [SerializeField] TMP_Text _separator;

    [Header("Standard text")]
    [SerializeField] string _labelingStandardResponse = "Thank you for your choice";

    [Header("Log elements")]
    [SerializeField] GameObject _container;
    [SerializeField] Transform _contentTransform;

    public enum LogType { AI, Player };
    public static script_ui_LabelLog Instance;

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

    public static void LogAIText(string logText, bool closeExchange = false)
    {
        LogExchangeElement(LogType.AI, logText, closeExchange);
    }

    public static void LogPlayerText(string logText, bool closeExchange = false)
    {
        LogExchangeElement(LogType.Player, logText, closeExchange);
    }

    public static void LogLabelingExchange(string labelingPrompt, string playerChoice)
    {
        ExchangeElement promptElement = new(LogType.AI, labelingPrompt);
        ExchangeElement choiceElement = new(LogType.Player, playerChoice);
        ExchangeElement standardResponseElement = new(LogType.AI, Instance?._labelingStandardResponse);

        LogExchange(new[]{ promptElement, choiceElement, standardResponseElement});
    }

    static void LogExchange(ExchangeElement[] exchange)
    {
        // goes through each element to log it one by one
        foreach (var exchangeElement in exchange)
        {
            var element = exchangeElement;
            TransformExchangeElementText(ref element);
            LogExchangeElement(element);
        }
        
        // closes off with separatorPrefab - every exchange is closed by the separator!
        // or do they start with separator, so there's only separator when there is something to separate from?
        CloseExchange();
        
    }

    static void LogExchangeElement(ExchangeElement element, bool closeExchange = false)
    {
        TMP_Text prefabToUse = Instance?.DetermineTextPrefab(element.type);

        TMP_Text textInstance = Instantiate(prefabToUse, Instance?._contentTransform);
        textInstance.text = element.text;

        if (closeExchange) CloseExchange();
    }
    public static void LogExchangeElement(LogType logType, string logText = "", bool closeExchange = false)
    {
        LogExchangeElement(new ExchangeElement(logType, logText), closeExchange);
    }

    public static void CloseExchange()
    {
        Instantiate(Instance?._separator, Instance?._contentTransform);
        // send signal to open log briefly
        // which signal interrupts any previous ones
    }

    TMP_Text DetermineTextPrefab(LogType type)
    {
        switch (type)
        {
            case LogType.AI:
                return _aiTextPrefab;
            case LogType.Player:
                return _playerTextPrefab;
            default:
                return null;
        }
    }

    static void TransformExchangeElementText(ref ExchangeElement element)
    {
        switch (element.type)
        {
            case LogType.AI:
                element.text = "> " + element.text;
                break;
            default:

                break;
        }
    }

    struct ExchangeElement
    {
        public LogType type;
        public string text;

        public ExchangeElement(LogType exchangeType, string exchangeText)
        {
            type = exchangeType;
            text = exchangeText;
        }
    }
}
