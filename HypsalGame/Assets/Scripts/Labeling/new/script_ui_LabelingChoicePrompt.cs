using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class script_ui_LabelingChoicePrompt : MonoBehaviour
{
    [SerializeField] TMP_Text _promptText;
    [SerializeField] TMP_Text _currentChoiceText;
    [SerializeField] Transform _choiceContainer;

    [SerializeField] script_ui_ChoicePrompt _choicePrefab;

    Label[] labels;

    int choiceCount;


    public void Initialize(string prompt, Label representedLabel, Label[] choiceLabels)
    {
        _promptText.text = prompt;

        _currentChoiceText.gameObject.SetActive(representedLabel.IsLabeled);
        if (representedLabel.IsLabeled)
        {
            _currentChoiceText.text = $"(Currently: {representedLabel.DisplayName})";
        }

            labels = choiceLabels;

        SetupChoices();

        gameObject.SetActive(true);
    }   
    
    void SetupChoices()
    {
        // shuffle later -- how to make that consistent between UI and 
        choiceCount = labels.Length;
        for (int i = 0; i < choiceCount; i++)
        {
            var choicePrompt = Instantiate(_choicePrefab, _choiceContainer);
            var label = labels[i];
            choicePrompt.Initialize(i + 1, label);
        }
    }


    public void Dispose()
    {
        Destroy(gameObject);
    }

}
