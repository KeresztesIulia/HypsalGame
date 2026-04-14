using TMPro;
using UnityEngine;

public class temp_script_LabelerTest : MonoBehaviour
{
    [SerializeField] script_so_NameList wordlist;
    [SerializeField] TMP_Text current;
    [SerializeField] TMP_Text translations;

    [SerializeField] TMP_InputField inputField;

    int currentWordIndex = 0;
    LabelableWord currentWord;

    private void Start()
    {
        wordlist.Init();
        currentWordIndex = 0;
        
        UpdateStrings();
    }

    void UpdateStrings()
    {
        currentWord = wordlist.GetWord(currentWordIndex);

        current.text = currentWord.originalName;
        ComputeTranslationList();
    }

    void ComputeTranslationList()
    {
        string translationsText = "";

        for (int i = 0; i < wordlist.wordCount; i++)
        {
            string original = wordlist.GetWord(i).originalName;
            string display = wordlist.GetWord(i).displayName;
            translationsText += $"{original}: {display}\n";
        }
        translations.text = translationsText;
    }

    public void OnTranslationSubmit()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;

        currentWord.LabelWord(inputField.text);

        currentWordIndex = NextWord();
        UpdateStrings();

        inputField.text = "";
    }

    int NextWord()
    {
        if (currentWordIndex + 1 == wordlist.wordCount)
        {
            return 0;
        }
        return currentWordIndex + 1;
    }
}
