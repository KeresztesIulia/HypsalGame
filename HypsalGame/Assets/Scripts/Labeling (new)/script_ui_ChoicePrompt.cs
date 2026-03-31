using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class script_ui_ChoicePrompt : MonoBehaviour
{
    [SerializeField] TMP_Text numberText;
    [SerializeField] TMP_Text labelText;
    [SerializeField] Button buttonComponent;

    public delegate void OnClickFunction(Label label);

    public void Initialize(int number, Label label, OnClickFunction onClick)
    {
        numberText.text = number.ToString();
        labelText.text = label.Name;
        buttonComponent.onClick.AddListener(() => onClick(label));

    }


}
