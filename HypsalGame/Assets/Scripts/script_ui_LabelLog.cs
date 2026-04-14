using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class script_ui_LabelLog : MonoBehaviour, interface_PersistentData, IScrollHandler
{
    [Header("Text prefabs")] // for formatting only
    [SerializeField] TMP_Text _aiTextPrefab;
    [SerializeField] TMP_Text _playerTextPrefab;
    [SerializeField] TMP_Text _separator;

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

    public enum LogType { AI, Player };
    public static script_ui_LabelLog Instance;

    bool initialized = false;

    public void Initialize()
    {
        Instance = this;
        script_InputManager.action_ShowLog.performed += (ctx) => ToggleLog();
        script_InputManager.action_PlayerScroll.performed += ScrollLog;
        initialized = true;
    }

    void Start()
    {
        if (!initialized) Initialize();
    }

    void ToggleLog(bool stopCoroutines = true)
    {

        if (stopCoroutines) StopAllCoroutines();
        _container.alpha = 1f;
        _container.gameObject.SetActive(!_container.gameObject.activeSelf);
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

        // scroll to bottom
        Instance?.StartCoroutine(Instance?.ForceToBottom());
    }
    public static void LogExchangeElement(LogType logType, string logText = "", bool closeExchange = false)
    {
        LogExchangeElement(new ExchangeElement(logType, logText), closeExchange);
    }

    public static void CloseExchange()
    {
        Instantiate(Instance?._separator, Instance?._contentTransform);
        Instance?.StartCoroutine(Instance?.ForceToBottom());

        // send signal to open log briefly
        // which signal interrupts any previous ones - does it...? I don't want to restart fading just because I'm already fading.
        if (Instance != null) Instance.StartCoroutine(Instance.ShowNewLog());
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

        public ExchangeElement(LogType exchangeType, string exchangeText)
        {
            type = exchangeType;
            text = exchangeText;
        }
    }
}
