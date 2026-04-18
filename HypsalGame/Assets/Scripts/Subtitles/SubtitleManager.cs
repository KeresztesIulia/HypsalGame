using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    [SerializeField] TMP_Text subtitleText;
    [SerializeField] GameObject subtitleCanvas;

    private bool isSubtitleActive = false;

    public enum SubtitleType { Generic };

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
       HideSubtitle();
    }

    public void ShowSubtitle(string text, float duration)
    {
        if (string.IsNullOrEmpty(text)) return;

        subtitleText.text = text;
        subtitleCanvas.SetActive(true);
        isSubtitleActive = true;

        CancelInvoke(nameof(HideSubtitle));
        Invoke(nameof(HideSubtitle), duration);
    }

    public void HideSubtitle()
    {
        subtitleCanvas.SetActive(false);
        subtitleText.text = "";
        isSubtitleActive = false;
    }
}
