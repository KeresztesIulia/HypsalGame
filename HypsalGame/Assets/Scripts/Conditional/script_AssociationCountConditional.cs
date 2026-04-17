using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class script_AssociationCountConditional : abstract_ConditionalRunner
{
    [SerializeField] bool _fixedValue; // have this many associations, or have this many more than when entered?
    [SerializeField] int _targetCount; // if not fixed value
    [SerializeField] Vector2Int _targetCountRange; // if fixed value; inclusive both

    int startValue = -1;

    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        script_LabelAssociationHandler.OnAnyAssociation.AddListener(() => ConditionMet = Condition());
    }

    protected override bool Condition()
    {
        int? currentCount = script_LabelAssociationHandler.Instance?.AssociationCount;
        if (_fixedValue) return currentCount >= _targetCountRange.x && currentCount <= _targetCountRange.y;
        if (startValue == -1) return false;

        if (_targetCount < 0) return currentCount <= startValue + _targetCount;
        else return currentCount >= startValue + _targetCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (startValue != -1 || _fixedValue) return;
        if (!script_LabelAssociationHandler.Instance) return;

        startValue = script_LabelAssociationHandler.Instance.AssociationCount;
    }
}
