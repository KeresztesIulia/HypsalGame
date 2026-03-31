using System;
using System.Collections.Generic;
using UnityEngine;

public class script_LabelRepresentative : script_Interactable
{
    [SerializeField] script_so_LabelList _partOfList;


    [SerializeField] string _representedLabelName;
    Label representedLabel;

    [SerializeField] string _prompt;

    [SerializeField] string[] _possibleAssociations;
    List<Label> possibleAssociationLabels;

    [SerializeField] bool _relabelable = false;

    public string Prompt => _prompt;
    public Label RepresentedLabel => representedLabel;
    public List<Label> PossibleLabels => possibleAssociationLabels;

    bool hasAssociations => possibleAssociationLabels != null && possibleAssociationLabels.Count > 0;

    bool labelable => hasAssociations && (_relabelable || representedLabel.Labelable); // should we allow override on the object itself? -> this actually does that and probably shouldn't
    // should it be able to show the "true" label, if we ever do it like that, in the possible list on relabeling? it should, right?

    private void Start()
    {
        if (_partOfList == null) return;
        representedLabel = _partOfList.GetLabel(_representedLabelName);
        possibleAssociationLabels = new List<Label>();
        foreach (var name in  _possibleAssociations)
        {
            if (string.IsNullOrEmpty(name)) continue;
            possibleAssociationLabels.Add(_partOfList.GetLabel(name));
        }
    }

    private void Update() // optimize
    {
        _uiInfo.name = representedLabel.DisplayName;
        if (!labelable)
        {
            _uiInfo.interactionWord = "";
        }
    }

    public override void Interact(Vector3 playerPosition)
    {
        FilterAssociations(); 
        if (!hasAssociations) Debug.Log("Cannot be labeled, can only be looked at"); // maybe separate logic, but then need stg instead of playerInteraction; further comments in that file
        else if (!labelable)
        {
            Debug.Log("Already labeled");
        }
        else
        {
            script_ui_LabelingChoiceHandler.InstantiatePrompt(this);
        }

        // new prompt system is pop-up for now
    }

    void FilterAssociations()
    {
        for (int i = 0; i < possibleAssociationLabels.Count; i++)
        {
            var association = possibleAssociationLabels[i];
            if (!association.Labelable)
            {
                possibleAssociationLabels.RemoveAt(i);
                i--;
            }
        }
    }
}
