using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(script_VisualObject))]
public class script_LabelRepresentative : MonoBehaviour, interface_Interactable, interface_PersistentData
{
    [SerializeField] script_so_LabelList _partOfList;

    [SerializeField] string _representedLabelName;
    Label representedLabel;

    [SerializeField] GameObject _representingModel;

    [SerializeField] string _prompt;

    [SerializeField] string[] _possibleAssociations;
    List<Label> possibleAssociationLabels;

    [SerializeField] bool _relabelable = false;

    public string Prompt => _prompt;
    public Label RepresentedLabel => representedLabel;
    public GameObject RepresentedModel => _representingModel;
    public List<Label> PossibleLabels => possibleAssociationLabels;

    bool hasAssociations => possibleAssociationLabels != null && FilteredAssociations().Length > 0;

    public bool labelable => !markedUnlabelable && hasAssociations && (_relabelable || representedLabel.Labelable); // should we allow override on the object itself? -> this actually does that and probably shouldn't

    public interface_Interactable.InteractionType interactionType => interface_Interactable.InteractionType.Labelable;

    public UnityEvent LabeledObject = new();

    script_VisualObject visual;

    bool markedUnlabelable = false;

    bool initialized = false;

    private void Start()
    {
        if (!initialized) Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        if (_partOfList == null) return;
        possibleAssociationLabels = new List<Label>();
        foreach (var name in _possibleAssociations)
        {
            if (string.IsNullOrEmpty(name)) continue;
            possibleAssociationLabels.Add(_partOfList.GetLabel(name));
        }

        visual = GetComponent<script_VisualObject>();
        visual.SetActiveState(false);

        InitializeRepresentedLabel(_representedLabelName);

        initialized = true;
    }

    void InitializeRepresentedLabel(string representedLabelName)
    {
        _representedLabelName = representedLabelName;
        representedLabel = _partOfList.GetLabel(representedLabelName);

        if (representedLabel == null) return;

        if (representedLabel.IsLabeled)
        {
            LabelLabeled();
        }

        representedLabel.Labeled.AddListener(LabelLabeled);

        representedLabel.Unlabeled.AddListener(Unlabeled);

        representedLabel.MarkedRelabelable.AddListener(Unlabeled);
    }

    void Unlabeled()
    {
        if (markedUnlabelable) return;
        visual.SetActiveState(false);
        enabled = true;
    }

    void LabelLabeled()
    {
        if (labelable) return;
        visual.SetActiveState(true);
        visual._uiInfo._objectName = representedLabel.DisplayName;
        enabled = false;
    }

    public Label[] FilteredAssociations()
    {
        return possibleAssociationLabels.Where(label =>  label.Labelable).ToArray();
    }

    public void MarkUnlabelable(string textToShow)
    {
        markedUnlabelable = true;

        if (!enabled) return;

        visual.SetActiveState(true);
        visual._uiInfo._objectName = textToShow;
        enabled = false;
    }

    void RemoveListeners()
    {
        if (representedLabel == null) return;

        representedLabel.Labeled.RemoveListener(LabelLabeled);
        representedLabel.Unlabeled.RemoveListener(Unlabeled);
        representedLabel.MarkedRelabelable.RemoveListener(Unlabeled);
    }

    public void SetRepresentedLabel(string representedLabelName)
    {
        RemoveListeners();
        InitializeRepresentedLabel(representedLabelName);
    }

    public void SetRepresentingModel(GameObject model)
    {
        _representingModel = model;
        if (representedLabel == null) return;
        script_LabelAssociationHandler.Instance?.ChangeAssociation(representedLabel, model); // ?
    }
}
