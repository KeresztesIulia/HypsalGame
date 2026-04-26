using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static SubtitleManager;

public class script_ui_LabelLog : MonoBehaviour, interface_PersistentData, IScrollHandler
{
    [Header("Text prefabs")] // for formatting only
    [SerializeField] TMP_Text _aiTextPrefab;
    [SerializeField] TMP_Text _playerTextPrefab;
    [SerializeField] TMP_Text _desperateAITextPrefab;
    [SerializeField] TMP_Text _separator;

    [Header("Subtitle text prefabs")] // again, only formatting
    [SerializeField] TMP_Text _subtitleGenericPrefab;
    [SerializeField] TMP_Text _subtitleStudentCPrefab;
    [SerializeField] TMP_Text _subtitleStudentDPrefab;
    [SerializeField] TMP_Text _subtitleStudentIPrefab;

    [Header("Standard text")]
    [SerializeField] string _labelingStandardResponse = "Thank you for your choice";

    [Header("Log elements")]
    [SerializeField] CanvasGroup _container;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Transform _contentTransform;

    [Header("Log fade settings")]
    [SerializeField] float _logFadeInTime = 0.3f;
    [SerializeField] float _logOpenTime = 4f;
    [SerializeField] float _logFadeOutTime = 0.15f;

    public enum LogType { Subtitle, AI, Player, DesperateAI };
    public static script_ui_LabelLog Instance;

    bool initialized = false;

    public void Initialize()
    {
        Instance = this;
        if (script_InputManager.Instance == null) return;
        script_InputManager.action_ShowLog.performed += (ctx) => ToggleLog();
        script_InputManager.action_ui_ShowLog.performed += (ctx) => ToggleLog();
        script_InputManager.action_ui_Cancel.performed += (ctx) => SetLogVisibility(false);
        script_InputManager.action_Cancel.performed += (ctx) => SetLogVisibility(false);
        script_InputManager.action_PlayerScroll.performed += ScrollLog;
        initialized = true;
    }

    void Start()
    {
        if (!initialized) Initialize();
    }

    public void ToggleLog(bool stopCoroutines = true)
    {
        SetLogVisibility(!_container.gameObject.activeSelf, stopCoroutines);
    }

    void ResetLogVisibility(bool stopCoroutines)
    {
        if (stopCoroutines) StopAllCoroutines();
        _container.alpha = 1f;
    }

    public void SetLogVisibility(bool active,  bool stopCoroutines = true)
    {
        ResetLogVisibility(stopCoroutines);
        _container.gameObject.SetActive(active);
    }


    void ScrollLog(InputAction.CallbackContext context)
    {
        if (!_scrollRect.gameObject.activeInHierarchy) return;
        float scrollAmount = context.ReadValue<Vector2>().y;
        var eventData = new PointerEventData(null);
        var scrollDelta = eventData.scrollDelta;
        scrollDelta.y = scrollAmount;
        eventData.scrollDelta = scrollDelta;

        _scrollRect.OnScroll(eventData);
    }

    public static void LogSubtitle(string logText, SubtitleType subtitleType, bool closeExchange = false)
    {
        LogSubtitleExchangeElement(subtitleType, logText);
        if (closeExchange) CloseExchange(false);
    }

    public static void LogAIText(string logText, bool closeExchange = false)
    {
        LogExchangeElement(LogType.AI, logText, closeExchange);
    }

    public static void LogDesperateAIText(string logText, bool closeExchange = false)
    {
        LogExchangeElement(LogType.DesperateAI, logText, closeExchange);
    }

    public static void LogPlayerText(string logText, bool closeExchange = false)
    {
        LogExchangeElement(LogType.Player, logText, closeExchange);
    }

    public static void LogLabelingExchange(string labelingPrompt, string playerChoice, string specialResponse = "")
    {
        ExchangeElement promptElement = new(LogType.AI, labelingPrompt);
        ExchangeElement choiceElement = new(LogType.Player, playerChoice);
        ExchangeElement responseElement = new(LogType.AI, string.IsNullOrEmpty(specialResponse) ? Instance?._labelingStandardResponse : specialResponse);

        LogExchange(new[]{ promptElement, choiceElement, responseElement });
    }

    static void LogExchange(ExchangeElement[] exchange)
    {
        // goes through each element to log it one by one
        foreach (var exchangeElement in exchange)
        {
            LogExchangeElement(exchangeElement);
        }
        
        // closes off with separatorPrefab - every exchange is closed by the separator!
        // or do they start with separator, so there's only separator when there is something to separate from?
        CloseExchange();
        
    }

    static void LogExchangeElement(ExchangeElement element, bool closeExchange = false)
    {
        TransformExchangeElementText(ref element);
        TMP_Text prefabToUse = Instance?.DetermineTextPrefab(element.type, element.subtitleType);

        TMP_Text textInstance = Instantiate(prefabToUse, Instance?._contentTransform);
        textInstance.text = element.text;

        if (closeExchange) CloseExchange();

        // scroll to bottom
        Instance?.StartCoroutine(Instance?.ForceToBottom());
    }
    public static void LogExchangeElement(LogType logType, string logText = "", bool closeExchange = false)
    {
        LogExchangeElement(new ExchangeElement(logType, logText), closeExchange);
    }

    public static void LogSubtitleExchangeElement(SubtitleType subtitleType, string logText = "", bool closeExchange = false)
    {
        var logType = LogType.Subtitle;
        LogExchangeElement(new ExchangeElement(logType, logText, subtitleType), closeExchange);
    }

    public static void CloseExchange(bool popup = true)
    {
        Instantiate(Instance?._separator, Instance?._contentTransform);

        Instance?.StartCoroutine(Instance?.ForceToBottom());

        // send signal to open log briefly
        // which signal interrupts any previous ones - does it...? I don't want to restart fading just because I'm already fading.
        if (Instance != null && popup) Instance.StartCoroutine(Instance.ShowNewLog());
    }

    TMP_Text DetermineTextPrefab(LogType type, SubtitleType subtitleType = SubtitleType.Generic)
    {
        switch (type)
        {
            case LogType.Subtitle:
                return DetermineTextPrefab(subtitleType);
            case LogType.AI:
                return _aiTextPrefab;
            case LogType.Player:
                return _playerTextPrefab;
            case LogType.DesperateAI:
                return _desperateAITextPrefab;
            default:
                return null;
        }
    }

    TMP_Text DetermineTextPrefab(SubtitleType subtitleType)
    {
        switch (subtitleType)
        {
            case (SubtitleType.StudentC):
                return _subtitleStudentCPrefab;
            case(SubtitleType.StudentD):
                return _subtitleStudentDPrefab;
            case(SubtitleType.StudentI):
                return _subtitleStudentIPrefab;
            default:
                return _subtitleGenericPrefab;
        }
    }

    static void TransformExchangeElementText(ref ExchangeElement element)
    {
        switch (element.type)
        {
            case LogType.Subtitle:
                element.text = "- " + element.text; // what if it's continued subtitle?
                break;
            case LogType.AI:
                element.text = "> " + element.text;
                break;
            default:

                break;
        }
    }

    IEnumerator ShowNewLog()
    {
        if (_container.gameObject.activeSelf) yield break;

        // fade in for x seconds
        _container.alpha = 0;
        ToggleLog(false);

        float currentTime = 0;
        while (currentTime <= _logFadeInTime)
        {
            _container.alpha = currentTime / _logFadeInTime;
            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _container.alpha = 1;

        // stay open for y seconds
        yield return new WaitForSecondsRealtime(_logOpenTime);

        // fade out for z seconds
        currentTime = _logFadeOutTime;
        while (currentTime >= 0)
        {
            _container.alpha = currentTime / _logFadeOutTime;
            currentTime -= Time.unscaledDeltaTime;
            yield return null;
        }
        ToggleLog(false);
    }

    IEnumerator ForceToBottom()
    {
        yield return new WaitForNextFrameUnit();
        yield return new WaitForEndOfFrame();
        _scrollRect.verticalNormalizedPosition = 0;
    }

    public void OnScroll(PointerEventData eventData)
    {
        ((IScrollHandler)_scrollRect).OnScroll(eventData);
    }

    struct ExchangeElement
    {
        public LogType type;
        public string text;

        public SubtitleType subtitleType;

        public ExchangeElement(LogType exchangeType, string exchangeText, SubtitleType exchangeSubtitleType = SubtitleType.Generic)
        {
            type = exchangeType;
            text = exchangeText;
            subtitleType = exchangeSubtitleType;
        }
    }
}
