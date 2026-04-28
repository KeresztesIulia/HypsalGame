using TMPro;
using UnityEngine;

public class script_SubtitleContainer : MonoBehaviour
{
    [SerializeField] TMP_Text[] subtitleTexts;

    public void ActivateContainer(string text)
    {
        ChangeSubtitle(text);

        gameObject.SetActive(true);
    }

    public void ChangeSubtitle(string text)
    {
        foreach (var subtitle in subtitleTexts)
        {
            subtitle.text = text;
        }
    }

    public void DeactivateContainer()
    {
        gameObject.SetActive(false);
    }
}
