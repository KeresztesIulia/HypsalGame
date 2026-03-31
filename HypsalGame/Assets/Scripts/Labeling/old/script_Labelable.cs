using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(script_Nameable))]
public class script_Labelable : script_Interactable
{
    [SerializeField] string _labelingPrompt = "Enter label:";

    LabelableWord representedWord;

    bool changedInteractionWord = false;
    bool labelable = true;

    void Start()
    {
        representedWord = GetComponent<script_Nameable>().RepresentedWord;
        labelable = true;
    }

    private void Update()
    {
        _uiInfo.name = representedWord.displayName;
        if (!changedInteractionWord && representedWord.named)
        {
            if (representedWord.renameable)
            {
                _uiInfo.interactionWord = "Relabel";
            }
            else
            {
                _uiInfo.interactionWord = "";
                labelable = false;
            }
            changedInteractionWord = true;
        }
    }

    public override void Interact(Vector3 playerPosition)
    {
        if (labelable)
        {
            script_ui_LabelingHandler.InstantiatePrompt(_labelingPrompt, representedWord);
        }
    }

}
