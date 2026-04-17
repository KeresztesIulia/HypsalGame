using UnityEngine;
using UnityEngine.Events;

public class script_AssociationConditional : abstract_ConditionalRunner
{
    [SerializeField] protected script_so_LabelList _labelList;
    [SerializeField] protected string _conditionLabelName1;
    [SerializeField] protected string _conditionLabelName2;

    protected Label conditionLabel1;
    protected Label conditionLabel2;

    // OnLabeling
    [SerializeField, Tooltip("Are there functions that should run every time the two condition labels are associated to each other?")] protected bool _checkAssociation;
    [SerializeField] protected UnityEvent _OnAssociationEvents;

    [SerializeField, Tooltip("Are there functions that should run every time one of the condition labels gets associated without fulfilling the condition?")] protected bool _checkAssociation_negative;
    [SerializeField] protected UnityEvent _OnAssociationNegativeEvents;

    protected override void Start()
    {
        if (_labelList == null) return;
        conditionLabel1 = _labelList.GetLabel(_conditionLabelName1);
        conditionLabel2 = _labelList.GetLabel(_conditionLabelName2);

        conditionLabel1.Associated += Label1AssociationCheck;
        conditionLabel2.Associated += Label2AssociationCheck;

        base.Start();
    }

    protected override bool Condition()
    {
        return script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);
    }

    protected void Label1AssociationCheck(Label label)
    {
        if (label == conditionLabel2)
        {
            _OnAssociationEvents?.Invoke();
            ConditionMet = true;
        }
        else
        {
            _OnAssociationNegativeEvents?.Invoke();
            ConditionMet = false;
        }
    }

    protected void Label2AssociationCheck(Label label)
    {
        if (label == conditionLabel1) return;

        _OnAssociationNegativeEvents.Invoke();
        ConditionMet = false;
    }

}
