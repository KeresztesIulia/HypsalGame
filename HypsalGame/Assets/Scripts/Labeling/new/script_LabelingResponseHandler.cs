using System;
using System.Collections.Generic;
using UnityEngine;


public class script_LabelingResponseHandler : MonoBehaviour, interface_PersistentData
{
    [SerializeField] script_so_LabelList _labelList;

    [SerializeField] string[] _firstLabels;
    [SerializeField] string[] _secondLabels;
    [SerializeField] string[] _responses;

    public static script_LabelingResponseHandler Instance;
    bool initialized = false;

    Dictionary<LabelPair, string> specialResponses;

    private void Start()
    {
        if (!initialized) Initialize();
    }
    public void Initialize()
    {
        if (initialized) return;

        specialResponses = new Dictionary<LabelPair, string>();
        for (int i = 0; i < _firstLabels.Length; i++)
        {
            Label firstLabel = _labelList.GetLabel(_firstLabels[i]);
            Label secondLabel = _labelList.GetLabel(_secondLabels[i]);

            specialResponses.Add(new(firstLabel, secondLabel), _responses[i]);
        }

        Instance = this;

        initialized = true;
    }

    public static string GetResponse(Label label1, Label label2)
    {
        if (Instance == null) return null;

        string response;
        Instance.specialResponses.TryGetValue(new(label1, label2), out response);

        return response;
    }     
}

class LabelPair : IEquatable<LabelPair>
{
    public Label label1;
    public Label label2;

    public LabelPair()
    {
        label1 = null;
        label2 = null;
    }

    public LabelPair(Label label1, Label label2)
    {
        this.label1 = label1;
        this.label2 = label2;
    }

    public static bool operator ==(LabelPair first, LabelPair second)
    {
        if (first is null && second is null) return true;
        if (first is null || second is null) return false;

        if (first.label1 == second.label1 && first.label2 == second.label2) return true;
        if (first.label1 == second.label2 && first.label2 == second.label1) return true;

        return false;
    }

    public static bool operator !=(LabelPair first, LabelPair second)
    {
        return !(first == second);
    }

    public bool Equals(LabelPair other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return label1.GetHashCode() ^ label2.GetHashCode();
    }
}
