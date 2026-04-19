using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    // Make one to match all of the different ones we need
    [SerializeField] script_SubtitleContainer _genericContainer;
    [SerializeField] script_SubtitleContainer _studentCContainer;
    [SerializeField] script_SubtitleContainer _studentDContainer;
    [SerializeField] script_SubtitleContainer _studentIContainer;


    private bool isSubtitleActive = false;

    public enum SubtitleType { Generic, StudentC, StudentD, StudentI };
    script_SubtitleContainer currentContainer = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
       HideAllSubtitles();
    }

    public void ShowSubtitle(SubtitleType type, string text, float duration)
    {
        if (string.IsNullOrEmpty(text)) return;

        SwapContainers(DetermineContainer(type), text);

        isSubtitleActive = true;

        CancelInvoke(nameof(HideSubtitle));
        Invoke(nameof(HideSubtitle), duration);
    }

    void SwapContainers(script_SubtitleContainer newContainer, string text)
    {
        if (newContainer.gameObject.activeInHierarchy)
        {
            newContainer.ChangeSubtitle(text);
        }
        else
        {
            currentContainer?.DeactivateContainer();
            newContainer.ActivateContainer(text);

            currentContainer = newContainer;
        }
    }

    public void HideSubtitle()
    {
        currentContainer?.gameObject.SetActive(false);
        currentContainer = null;
        isSubtitleActive = false;
    }

    public void HideAllSubtitles()
    {
        _genericContainer.DeactivateContainer();
        _studentCContainer.DeactivateContainer();
        _studentDContainer.DeactivateContainer();
        _studentIContainer.DeactivateContainer();

        currentContainer = null;
        isSubtitleActive = false;
    }

    script_SubtitleContainer DetermineContainer(SubtitleType subtitleType)
    {
        switch (subtitleType)
        {

            case SubtitleType.StudentC:
                return _studentCContainer;
            case SubtitleType.StudentD:
                return _studentDContainer;
            case SubtitleType.StudentI:
                return _studentIContainer;
            default:
                return _genericContainer;
        }
    }
}
