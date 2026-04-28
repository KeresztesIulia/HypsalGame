using UnityEngine;
using UnityEngine.Events;

public class script_OLD_ConditionalRunner : MonoBehaviour
{
    [SerializeField] script_so_LabelList _labelList;
    [SerializeField] string _conditionLabelName1;
    [SerializeField] string _conditionLabelName2;

    Label conditionLabel1;
    Label conditionLabel2;

    // Continuous
    [SerializeField, Tooltip("Are there functions that should run every frame the condition is true?")] bool _checkContinuously;
    [SerializeField] UnityEvent _ContinuousEvents;

    [SerializeField, Tooltip("Are there functions that should run every frame the condition ISN'T true?")] bool _checkContinuously_negative;
    [SerializeField] UnityEvent _ContinuousNegativeEvents;

    // OnChange
    [SerializeField, Tooltip("Are there functions that should run every time the condition switches from unfulfilled to fulfilled?")] bool _checkChange;
    [SerializeField] UnityEvent _OnChangeEvents;

    [SerializeField, Tooltip("Are there functions that should run every time the condition switches from fulfilled to unfulfilled?")] bool _checkChange_negative;
    [SerializeField] UnityEvent _OnChangeNegativeEvents;

    // FireOnce
    [SerializeField, Tooltip("Are there functions that should run the first time the condition switches from unfulfilled to fulfilled?")] bool _fireOnce;
    [SerializeField] UnityEvent _FireOnceEvents;

    [SerializeField, Tooltip("Are there functions that should run the first time the condition switches from fulfilled to unfulfilled?")] bool _fireOnce_negative;
    [SerializeField] UnityEvent _FireOnceNegativeEvents;

    // AtStart
    [SerializeField, Tooltip("Are there functions that should run if the condition is true when the object first appears?")] bool _checkAtStart;
    [SerializeField] UnityEvent _AtStartEvents;

    [SerializeField, Tooltip("Are there functions that should run if the condition is false when the object first appears?")] bool _checkAtStart_negative;
    [SerializeField] UnityEvent _AtStartNegativeEvents;

    // OnLabeling
    [SerializeField, Tooltip("Are there functions that should run every time the two condition labels are associated to each other?")] bool _checkAssociation;
    [SerializeField] UnityEvent _OnAssociationEvents;

    [SerializeField, Tooltip("Are there functions that should run every time one of the condition labels gets associated without fulfilling the condition?")] bool _checkAssociation_negative;
    [SerializeField] UnityEvent _OnAssociationNegativeEvents;


    bool conditionMet;

    public bool ConditionMet
    {
        get
        {
            return conditionMet;
        }
        private set
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
                if (_checkContinuously_negative && !value) _OnChangeNegativeEvents?.Invoke();
            }

            conditionMet = value;
        }
    }

    bool hasContinuous => _checkContinuously || _checkContinuously_negative;
    bool hasOnChange => _checkChange || _checkChange_negative;
    bool hasFireOnce => _fireOnce || _fireOnce_negative;

    bool fired = false;
    bool firedNegative = false;

    bool firedBoth => fired && firedNegative;
     
    private void Start()
    {
        Debug.LogError("ConditionalRunner is now an old version. Use AssociationConditional instead. (Will require setting up events again)", this);
        if (_labelList == null) return;
        conditionLabel1 = _labelList.GetLabel(_conditionLabelName1);
        conditionLabel2 = _labelList.GetLabel(_conditionLabelName2);

        conditionMet = script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);

        if (_checkAtStart && conditionMet) _AtStartEvents?.Invoke();
        if (_checkAtStart_negative && !conditionMet) _AtStartNegativeEvents?.Invoke();

        conditionLabel1.Associated += Label1AssociationCheck;
        conditionLabel2.Associated += Label2AssociationCheck;

    }

    private void Update()
    {
        if (script_LabelAssociationHandler.Instance == null) return;
        
        if (!hasContinuous) return;


        ConditionMet = script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);


        if (_checkContinuously && ConditionMet) _ContinuousEvents.Invoke();
        if (_checkContinuously_negative && ConditionMet) _ContinuousNegativeEvents.Invoke();
    }

    void Label1AssociationCheck(Label label)
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

    void Label2AssociationCheck(Label label)
    {
        if (label == conditionLabel1) return;
        
        _OnAssociationNegativeEvents.Invoke();
        ConditionMet = false;
    }

    public void DebugMessage(string message)
    {
        Debug.Log(message); 
    }
}
