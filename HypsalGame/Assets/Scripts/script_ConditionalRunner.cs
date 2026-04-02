using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class script_ConditionalRunner : MonoBehaviour
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
        if (_labelList == null) return;
        conditionLabel1 = _labelList.GetLabel(_conditionLabelName1);
        conditionLabel2 = _labelList.GetLabel(_conditionLabelName2);

        conditionMet = script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);

        if (_checkAtStart && conditionMet) _AtStartEvents?.Invoke();
        if (_checkAtStart_negative && !conditionMet) _AtStartNegativeEvents?.Invoke();
    }

    private void Update()
    {
        if (script_LabelAssociationHandler.Instance == null) return;
        
        if (!hasContinuous && !hasOnChange && (!_fireOnce || fired) && (!_fireOnce_negative || firedNegative)) return;


        ConditionMet = script_LabelAssociationHandler.Instance.AreAssociated(conditionLabel1, conditionLabel2);
        // have an association trigger instead and subscribe to it? -- later


        if (_checkContinuously && ConditionMet) _ContinuousEvents.Invoke();
        if (_checkContinuously_negative && ConditionMet) _ContinuousNegativeEvents.Invoke();
    }

    public void DebugMessage(string message)
    {
        Debug.Log(message); 
    }
}
