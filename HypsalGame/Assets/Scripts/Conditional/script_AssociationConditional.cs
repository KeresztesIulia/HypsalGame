public class script_AssociationConditional : abstract_ConditionalRunner
{
    protected override bool Condition()
    {
        return script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);
    }

    protected override void Label1AssociationCheck(Label label)
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

    protected override void Label2AssociationCheck(Label label)
    {
        if (label == conditionLabel1) return;

        _OnAssociationNegativeEvents.Invoke();
        ConditionMet = false;
    }

}
