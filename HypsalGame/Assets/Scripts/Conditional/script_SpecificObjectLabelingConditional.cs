using UnityEngine;

public class script_SpecificObjectLabelingConditional : abstract_ConditionalRunner
{
    [SerializeField] protected script_LabelRepresentative[] _labelableObjects;
    [SerializeField] protected int _numberOfObjectsToLabel;

    protected bool[] labeled;

    protected override void Start()
    {

        if (_labelableObjects == null) return;

        labeled = new bool[_labelableObjects.Length];

        for (int i = 0; i < _labelableObjects.Length; i++)
        {
            int idx = i;
            var labelableObject = _labelableObjects[i];
            labelableObject.LabeledObject.AddListener(() => AddLabel(idx));
            labelableObject.RepresentedLabel.Unlabeled.AddListener(() => RemoveLabel(idx));
        }

        base.Start();

    }

    protected override bool Condition()
    {
        if (_numberOfObjectsToLabel == 0) return false;
        int sum = 0;
        foreach (var labeledValue in labeled) sum += labeledValue ? 1 : 0;
        return sum >= _numberOfObjectsToLabel;
    }
    void AddLabel(int idx)
    {
        labeled[idx] = true;
        ConditionMet = Condition();
    }

    void RemoveLabel(int idx)
    {
        labeled[idx] = false;
        ConditionMet = Condition();
    }
}
