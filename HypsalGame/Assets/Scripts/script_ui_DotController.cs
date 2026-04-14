using UnityEngine;

public class script_ui_DotController : MonoBehaviour, interface_PersistentData
{
    [SerializeField] GameObject _centerDot;
    [SerializeField] GameObject _interactableDot;

    public static script_ui_DotController Instance;

    bool initialized = false;

    private void Start()
    {
        if (!initialized) Initialize();
    }
    public void Initialize()
    {
        if (initialized) return;

        Instance = this;
    }

    public static void SetCenterDotState(bool state)
    {
        Instance?._centerDot?.SetActive(state);

    }

    public static void SetInteractableDotState(bool state)
    {
        Instance?._interactableDot?.SetActive(state);
    }

    public static void DisableDot()
    {
        SetInteractableDotState(false);
        SetCenterDotState(false);

    }

}
