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

    bool isLabeled = false; // uh... shouldn't this just be a getter based on givenName? anyway, revise when we have AssociationList

    public string Name => _originalName;
    public string DisplayName => isLabeled ? givenName : "???";
    public string ForcedDisplayName => isLabeled ? givenName : _originalName;

    public bool IsLabeled => isLabeled;

    public bool Labelable => _relabelable || !IsLabeled;

    public void Reset()
    {
        isLabeled = false;
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
        // replace this with simply checking associations actually. Not gonna store these here
        givenName = givenLabel;
        isLabeled = true;
    }

    public void SetLabel(Label label)
    {
        SetLabel(label.Name);
        label.SetLabel(Name);

        // disassociate previous association, if it exists
        // associate labels
    }

    public override string ToString()
    {
        return ForcedDisplayName;
    }

    public static implicit operator string(Label label)
    {
        return label.ToString();
    }
}

