using UnityEngine;
using UnityEngine.Events;

public abstract class abstract_ConditionalRunner : MonoBehaviour
{
    [SerializeField] protected script_so_LabelList _labelList;
    [SerializeField] protected string _conditionLabelName1;
    [SerializeField] protected string _conditionLabelName2;

    protected Label conditionLabel1;
    protected Label conditionLabel2;

    // Continuous
    [SerializeField, Tooltip("Are there functions that should run every frame the condition is true?")] protected bool _checkContinuously;
    [SerializeField] protected UnityEvent _ContinuousEvents;

    [SerializeField, Tooltip("Are there functions that should run every frame the condition ISN'T true?")] protected bool _checkContinuously_negative;
    [SerializeField] protected UnityEvent _ContinuousNegativeEvents;

    // OnChange
    [SerializeField, Tooltip("Are there functions that should run every time the condition switches from unfulfilled to fulfilled?")] protected bool _checkChange;
    [SerializeField] protected UnityEvent _OnChangeEvents;

    [SerializeField, Tooltip("Are there functions that should run every time the condition switches from fulfilled to unfulfilled?")] protected bool _checkChange_negative;
    [SerializeField] protected UnityEvent _OnChangeNegativeEvents;

    // FireOnce
    [SerializeField, Tooltip("Are there functions that should run the first time the condition switches from unfulfilled to fulfilled?")] protected bool _fireOnce;
    [SerializeField] protected UnityEvent _FireOnceEvents;

    [SerializeField, Tooltip("Are there functions that should run the first time the condition switches from fulfilled to unfulfilled?")] protected bool _fireOnce_negative;
    [SerializeField] protected UnityEvent _FireOnceNegativeEvents;

    // AtStart
    [SerializeField, Tooltip("Are there functions that should run if the condition is true when the object first appears?")] protected bool _checkAtStart;
    [SerializeField] protected UnityEvent _AtStartEvents;

    [SerializeField, Tooltip("Are there functions that should run if the condition is false when the object first appears?")] protected bool _checkAtStart_negative;
    [SerializeField] protected UnityEvent _AtStartNegativeEvents;

    // OnLabeling
    [SerializeField, Tooltip("Are there functions that should run every time the two condition labels are associated to each other?")] protected bool _checkAssociation;
    [SerializeField] protected UnityEvent _OnAssociationEvents;

    [SerializeField, Tooltip("Are there functions that should run every time one of the condition labels gets associated without fulfilling the condition?")] protected bool _checkAssociation_negative;
    [SerializeField] protected UnityEvent _OnAssociationNegativeEvents;


    protected bool conditionMet;

    public bool ConditionMet
    {
        get
        {
            return conditionMet;
        }
        protected set
        {
            if (value != conditionMet)
            {
                if (_fireOnce && !fired && value)
                {
                    fired = true;
                    _FireOnceEvents?.Invoke();
                }
                if (_fireOnce_negative && !firedNegative && !value)
                {
                    firedNegative = true;
                    _FireOnceNegativeEvents?.Invoke();
                }

                if (_checkChange && value) _OnChangeEvents?.Invoke();
                if (_checkChange_negative && !value) _OnChangeNegativeEvents?.Invoke();
            }

            conditionMet = value;
        }
    }

    protected bool hasContinuous => _checkContinuously || _checkContinuously_negative;
    protected bool hasOnChange => _checkChange || _checkChange_negative;
    protected bool hasFireOnce => _fireOnce || _fireOnce_negative;

    protected bool fired = false;
    protected bool firedNegative = false;

    protected bool firedBoth => fired && firedNegative;

    protected void Start()
    {
        if (_labelList == null) return;
        conditionLabel1 = _labelList.GetLabel(_conditionLabelName1);
        conditionLabel2 = _labelList.GetLabel(_conditionLabelName2);

        conditionMet = Condition();

        if (_checkAtStart && conditionMet) _AtStartEvents?.Invoke();
        if (_checkAtStart_negative && !conditionMet) _AtStartNegativeEvents?.Invoke();

        conditionLabel1.Associated += Label1AssociationCheck;
        conditionLabel2.Associated += Label2AssociationCheck;

    }

    protected void Update()
    {
        if (script_LabelAssociationHandler.Instance == null) return;

        if (!hasContinuous) return;


        ConditionMet = script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);


        if (_checkContinuously && ConditionMet) _ContinuousEvents.Invoke();
        if (_checkContinuously_negative && ConditionMet) _ContinuousNegativeEvents.Invoke();
    }

    protected abstract bool Condition();

    protected abstract void Label1AssociationCheck(Label label);

    protected abstract void Label2AssociationCheck(Label label);

    public void DebugMessage(string message)
    {
        Debug.Log(message);
    }
}
