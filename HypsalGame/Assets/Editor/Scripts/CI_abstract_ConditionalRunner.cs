using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(abstract_ConditionalRunner))]  
public abstract class CI_abstract_ConditionalRunner : Editor
{
    public override void OnInspectorGUI()
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(this), GetType(), false);
        }

        bool shouldContinue = InitialSetup();

        if (!shouldContinue) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Condition", EditorStyles.boldLabel);
        SetUpConditionField();
        EditorGUILayout.Space();

        SetUpBasicEvents();
        EditorGUILayout.Space();
        SetUpExtraEvents();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual bool InitialSetup() { return true; }
    protected abstract void SetUpConditionField();

    void SetUpBasicEvents()
    {
        var continuousProp = serializedObject.FindProperty("_checkContinuously");
        var continuouNegativesProp = serializedObject.FindProperty("_checkContinuously_negative");
        var changeProp = serializedObject.FindProperty("_checkChange");
        var changeNegativeProp = serializedObject.FindProperty("_checkChange_negative");
        var fireOnceProp = serializedObject.FindProperty("_fireOnce");
        var fireOnceNegativeProp = serializedObject.FindProperty("_fireOnce_negative");
        var atStartProp = serializedObject.FindProperty("_checkAtStart");
        var atStartNegativeProp = serializedObject.FindProperty("_checkAtStart_negative");

        atStartProp.boolValue = EditorGUILayout.Toggle("Check at start", atStartProp.boolValue);
        if (atStartProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartEvents"));

        EditorGUILayout.Space();

        atStartNegativeProp.boolValue = EditorGUILayout.Toggle("Check at start (negative)", atStartNegativeProp.boolValue);
        if (atStartNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartNegativeEvents"));

        EditorGUILayout.Space();

        changeProp.boolValue = EditorGUILayout.Toggle("Check change", changeProp.boolValue);
        if (changeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeEvents"));

        EditorGUILayout.Space();

        changeNegativeProp.boolValue = EditorGUILayout.Toggle("Check change (negative)", changeNegativeProp.boolValue);
        if (changeNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeNegativeEvents"));

        EditorGUILayout.Space();

        continuousProp.boolValue = EditorGUILayout.Toggle("Check continuously", continuousProp.boolValue);
        if (continuousProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousEvents"));

        EditorGUILayout.Space();

        continuouNegativesProp.boolValue = EditorGUILayout.Toggle("Check continuously (negative)", continuouNegativesProp.boolValue);
        if (continuouNegativesProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousNegativeEvents"));

        EditorGUILayout.Space();

        fireOnceProp.boolValue = EditorGUILayout.Toggle("Check first change", fireOnceProp.boolValue);
        if (fireOnceProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceEvents"));

        EditorGUILayout.Space();

        fireOnceNegativeProp.boolValue = EditorGUILayout.Toggle("Check first change (negative)", fireOnceNegativeProp.boolValue);
        if (fireOnceNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceNegativeEvents"));
    }

    protected virtual void SetUpExtraEvents() { }
}
