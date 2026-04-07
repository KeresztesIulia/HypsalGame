using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(script_VisualObject))]
public class script_LabelRepresentative : MonoBehaviour, interface_Interactable
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

    bool hasAssociations => possibleAssociationLabels != null && FilteredAssociations().Length > 0;

    public bool labelable => hasAssociations && (_relabelable || representedLabel.Labelable); // should we allow override on the object itself? -> this actually does that and probably shouldn't

    public interface_Interactable.InteractionType interactionType => interface_Interactable.InteractionType.Labelable;

    script_VisualObject visual;

    private void Start()
    {
        if (_partOfList == null) return;
        representedLabel = _partOfList.GetLabel(_representedLabelName);
        possibleAssociationLabels = new List<Label>();
        foreach (var name in _possibleAssociations)
        {
            if (string.IsNullOrEmpty(name)) continue;
            possibleAssociationLabels.Add(_partOfList.GetLabel(name));
        }
        
        visual = GetComponent<script_VisualObject>();
        visual.SetActiveState(false);

        representedLabel.Labeled.AddListener(() =>
        {
            if (labelable) return;
            visual.SetActiveState(true);
            visual._uiInfo._objectName = representedLabel.DisplayName;
            enabled = false;
        });

        representedLabel.Unlabeled.AddListener(() =>
        {
            visual.SetActiveState(false);
            enabled = true;
        });
    }

    public Label[] FilteredAssociations()
    {
        return possibleAssociationLabels.Where(label =>  label.Labelable).ToArray();
    }
}
