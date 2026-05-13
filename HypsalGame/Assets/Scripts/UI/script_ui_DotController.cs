using UnityEngine;

public class script_ui_DotController : MonoBehaviour, interface_PersistentData
{
    [Header("Dot References")]
    [SerializeField] GameObject _centerDot;
    [SerializeField] GameObject _interactableDot;

    [Header("Mouse Idle Fade")]
    [Tooltip("When enabled, the dots fade out after the mouse has been idle for the specified amount of time.")]
    [SerializeField] bool _useMouseIdleFade = true;

    [Tooltip("Seconds without mouse input before the dots begin fading out.")]
    [SerializeField, Min(0f)] float _secondsWithoutMouseInputBeforeFade = 2f;

    [Tooltip("Seconds it takes the dots to fade from 100% opacity to 0% opacity once fading starts.")]
    [SerializeField, Min(0f)] float _fadeToTransparentDuration = 1f;

    public static script_ui_DotController Instance;

    CanvasGroup _centerDotCanvasGroup;
    CanvasGroup _interactableDotCanvasGroup;

    Vector3 _lastMousePosition;
    float _lastMouseInputTime;
    float _currentDotOpacity = 1f;
    bool _hasLastMousePosition = false;
    bool initialized = false;

    private void Start()
    {
        if (!initialized) Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        Instance = this;

        _centerDotCanvasGroup = GetOrAddCanvasGroup(_centerDot);
        _interactableDotCanvasGroup = GetOrAddCanvasGroup(_interactableDot);

        _lastMousePosition = Input.mousePosition;
        _lastMouseInputTime = Time.unscaledTime;
        _hasLastMousePosition = true;

        SetDotOpacity(1f);

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) Initialize();

        if (!_useMouseIdleFade)
        {
            _lastMouseInputTime = Time.unscaledTime;
            _lastMousePosition = Input.mousePosition;
            _hasLastMousePosition = true;
            SetDotOpacity(1f);
            return;
        }

        if (HasMouseInput())
        {
            _lastMouseInputTime = Time.unscaledTime;
            SetDotOpacity(1f);
            return;
        }

        float timeWithoutMouseInput = Time.unscaledTime - _lastMouseInputTime;

        if (timeWithoutMouseInput <= _secondsWithoutMouseInputBeforeFade)
        {
            SetDotOpacity(1f);
            return;
        }

        if (_fadeToTransparentDuration <= 0f)
        {
            SetDotOpacity(0f);
            return;
        }

        float timeSpentFading = timeWithoutMouseInput - _secondsWithoutMouseInputBeforeFade;
        float fadeProgress = Mathf.Clamp01(timeSpentFading / _fadeToTransparentDuration);

        SetDotOpacity(1f - fadeProgress);
    }

    bool HasMouseInput()
    {
        bool movedByAxis = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.01f ||
                           Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.01f;

        bool movedByPosition = false;
        Vector3 currentMousePosition = Input.mousePosition;

        if (_hasLastMousePosition)
        {
            movedByPosition = (currentMousePosition - _lastMousePosition).sqrMagnitude > 0.01f;
        }

        _lastMousePosition = currentMousePosition;
        _hasLastMousePosition = true;

        bool clickedOrHeld = Input.GetMouseButton(0) ||
                             Input.GetMouseButton(1) ||
                             Input.GetMouseButton(2);

        bool scrolled = Input.mouseScrollDelta.sqrMagnitude > 0.01f;

        return movedByAxis || movedByPosition || clickedOrHeld || scrolled;
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject dotObject)
    {
        if (dotObject == null) return null;

        CanvasGroup canvasGroup = dotObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = dotObject.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
    }

    void SetDotOpacity(float opacity)
    {
        _currentDotOpacity = Mathf.Clamp01(opacity);

        if (_centerDotCanvasGroup != null)
        {
            _centerDotCanvasGroup.alpha = _currentDotOpacity;
        }

        if (_interactableDotCanvasGroup != null)
        {
            _interactableDotCanvasGroup.alpha = _currentDotOpacity;
        }
    }

    public static void SetCenterDotState(bool state)
    {
        if (Instance == null) return;

        Instance._centerDot?.SetActive(state);
        Instance.SetDotOpacity(Instance._currentDotOpacity);
    }

    public static void SetInteractableDotState(bool state)
    {
        if (Instance == null) return;

        Instance._interactableDot?.SetActive(state);
        Instance.SetDotOpacity(Instance._currentDotOpacity);
    }

    public static void DisableDot()
    {
        SetInteractableDotState(false);
        SetCenterDotState(false);
    }
}