using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "so_LabelList", menuName = "Scriptable Objects/Label List")]
public class script_so_LabelList : ScriptableObject
{
    [SerializeField] List<Label> objects;
    [SerializeField] List<Label> labels;

    public List<Label> Labels => labels;
    public List<string> LabelStrings => objects.Concat(labels).Select(label => label.InternalName).ToList();

    public void Reset()
    {
        if (labels != null)
        foreach (var label in labels)
        {
            label.Reset();
        }

        if (objects != null)
        foreach (var obj in objects)
            { obj.Reset(); }    

    }

    public int GetLabelIndex(Label label)
    {
        int i = 0;
        int j = 0;
        for (i = 0; i < objects.Count; i++)
        {
            if (label == objects[i]) return i;
        }
        for (j = 0; j < labels.Count; j++)
        {
            if (label == labels[j]) return i + j;
        }
        return -1;
    }

    public int GetLabelIndex(string labelName)
    {
        int i = 0;
        int j = 0;
        for (i = 0; i < objects.Count; i++)
        {
            if (labelName == objects[i]) return i;
        }
        for (j = 0; j < labels.Count; j++)
        {
            if (labelName == labels[j]) return i + j;
        }
        return -1;
    }

    public Label GetLabel(int index)
    {
        if (index < 0) return null;
        if (index < objects.Count) return objects[index];
        if (index >= objects.Count + labels.Count) return null;
        return labels[index - objects.Count];
    }

    public Label GetLabel(string labelName)
    {
        return GetLabel(GetLabelIndex(labelName));
    }

}

[Serializable]
public class Label : IEquatable<Label>, IEquatable<string>
{
    [SerializeField] string _internalName;
    [SerializeField] string _displayName;
    string givenName = "";
    [SerializeField] bool _relabelable = false;

    [HideInInspector] public UnityEvent Labeled = new();
    [HideInInspector] public UnityEvent Unlabeled = new();

    [HideInInspector] public Action<Label> Associated = delegate { };

    public string InternalName => _internalName;
    public string OriginalDisplayName => string.IsNullOrEmpty(_displayName) ? _internalName : _displayName;

    public string DisplayName
    {
        get
        {
            if (IsLabeled)
            {
                if (script_LabelAssociationHandler.InstanceExists)
                {
                    var association = script_LabelAssociationHandler.Instance?.FindAssociatedLabel(this);
                    return association.OriginalDisplayName;
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
    public string ForcedDisplayName => IsLabeled ? DisplayName : OriginalDisplayName;

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
        return _internalName.Equals(other);
    }

    public static bool operator ==(Label first, Label second)
    {
        if (first is null && second is null) return true;
        if (first is null && second is not null) return false;
        if (first is not null && second is null) return false;
        return first.InternalName == second.InternalName;
    }

    public static bool operator !=(Label first, Label second)
    {
        return !(first == second);
    }

    public static bool operator ==(Label label, string name)
    {
        return label.InternalName == name;
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
            SetLabel(label.InternalName);
            label.SetLabel(InternalName);
        }
        Labeled?.Invoke();
    }

    public override string ToString()
    {
        return InternalName; // because this should be for quick usage and so should actually describe the object. Anything else should be deliberate.
    }

    public static implicit operator string(Label label)
    {
        return label.ToString();
    }
}

