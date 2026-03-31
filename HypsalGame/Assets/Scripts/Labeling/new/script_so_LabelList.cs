using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "so_LabelList", menuName = "Scriptable Objects/Label List")]
public class script_so_LabelList : ScriptableObject
{
    [SerializeField] List<Label> labels;

    public List<Label> Labels => labels;
    public List<string> LabelStrings => labels.Select(label => label.Name).ToList();

    public void Reset()
    {

        foreach (var label in Labels)
        {
            label.Reset();
        }

    }


    public int GetLabelIndex(Label label)
    {
        for (int i = 0; i < labels.Count; i++)
        {
            if (label == labels[i]) return i;
        }
        return -1;
    }

    public int GetLabelIndex(string labelName)
    {
        for (int i = 0; i < labels.Count; i++)
        {
            if (labels[i] == labelName) return i;
        }
        return -1;
    }

    public Label GetLabel(int index)
    {
        if (index < 0 || index >= labels.Count) return null;
        return labels[index];
    }

    public Label GetLabel(string labelName)
    {
        return GetLabel(GetLabelIndex(labelName));
    }

}

[Serializable]
public class Label : IEquatable<Label>, IEquatable<string>
{
    [SerializeField] string _originalName;
    string givenName = "";
    [SerializeField] bool _relabelable = false;

    public string Name => _originalName;
    public string DisplayName
    {
        get
        {
            if (IsLabeled)
            {
                if (script_LabelAssociationHandler.InstanceExists)
                {
                    var association = script_LabelAssociationHandler.Instance?.FindAssociatedLabel(this);
                    return association.Name;
                }
                else
                {
                    return givenName;
                }
            }
            else
            {
                return "???";
            }
        }
    }
    public string ForcedDisplayName => IsLabeled ? DisplayName : _originalName;

    public bool IsLabeled => script_LabelAssociationHandler.InstanceExists
        ? script_LabelAssociationHandler.Instance.HasAssociation(this) 
        : !string.IsNullOrEmpty(givenName);

    public bool Labelable => _relabelable || !IsLabeled;

    public void Reset()
    {
        givenName = "";
    }

    public bool Equals(Label other)
    {
        return this == other;
    }

    public bool Equals(string other)
    {
        return _originalName.Equals(other);
    }

    public static bool operator ==(Label first, Label second)
    {
        if (first is null && second is null) return true;
        if (first is null && second is not null) return false;
        if (first is not null && second is null) return false;
        return first.Name == second.Name;
    }

    public static bool operator !=(Label first, Label second)
    {
        return !(first == second);
    }

    public static bool operator ==(Label label, string name)
    {
        return label.Name == name;
    }

    public static bool operator !=(Label label, string name)
    {
        return !(label == name);
    }

    public void SetLabel(string givenLabel)
    {
        givenName = givenLabel;
    }

    public void SetLabel(Label label)
    {
        
        if (script_LabelAssociationHandler.InstanceExists)
        {
            script_LabelAssociationHandler.Instance?.AddAssociation(this, label);
            Debug.Log($"associating {this} with {label}");
        }
        else
        {
            SetLabel(label.Name);
            label.SetLabel(Name);
        }

    }

    public override string ToString()
    {
        return Name; // because this should be for quick usage and so should actually describe the object. Anything else should be deliberate.
    }

    public static implicit operator string(Label label)
    {
        return label.ToString();
    }
}

