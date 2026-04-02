using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(script_ConditionalRunner))]  
public class CI_ConditionalRunner : Editor
{
    public override void OnInspectorGUI()
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(this), GetType(), false);
        }

        var labelListProp = serializedObject.FindProperty("_labelList");
        EditorGUILayout.PropertyField(labelListProp);

        var labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList == null) return;

        SetUpConditionField(labelList);

        SetUpEvents();

        serializedObject.ApplyModifiedProperties();
    }

    void SetUpConditionField(script_so_LabelList labelList)
    {
        var conditionLabel1Prop = serializedObject.FindProperty("_conditionLabelName1");
        var conditionLabel2Prop = serializedObject.FindProperty("_conditionLabelName2");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Condition", EditorStyles.boldLabel, GUILayout.Width(75));
        conditionLabel1Prop.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, conditionLabel1Prop.stringValue);
        EditorGUILayout.LabelField(" = ", GUILayout.Width(15));
        conditionLabel2Prop.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, conditionLabel2Prop.stringValue);

        EditorGUILayout.EndHorizontal();
    }

    void SetUpEvents()
    {
        var continuousProp = serializedObject.FindProperty("_checkContinuously");
        var continuouNegativesProp = serializedObject.FindProperty("_checkContinuously_negative");
        var changeProp = serializedObject.FindProperty("_checkChange");
        var changeNegativeProp = serializedObject.FindProperty("_checkChange_negative");
        var fireOnceProp = serializedObject.FindProperty("_fireOnce");
        var fireOnceNegativeProp = serializedObject.FindProperty("_fireOnce_negative");
        var atStartProp = serializedObject.FindProperty("_checkAtStart");
        var atStartNegativeProp = serializedObject.FindProperty("_checkAtStart_negative");

        atStartProp.boolValue = EditorGUILayout.BeginToggleGroup("Check at start", atStartProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartEvents"));
        EditorGUILayout.EndToggleGroup();

        atStartNegativeProp.boolValue = EditorGUILayout.BeginToggleGroup("Check at start (negative)", atStartNegativeProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartNegativeEvents"));
        EditorGUILayout.EndToggleGroup();

        changeProp.boolValue = EditorGUILayout.BeginToggleGroup("Check change", changeProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeEvents"));
        EditorGUILayout.EndToggleGroup();

        changeNegativeProp.boolValue = EditorGUILayout.BeginToggleGroup("Check change (negative)", changeNegativeProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeNegativeEvents"));
        EditorGUILayout.EndToggleGroup();

        continuousProp.boolValue = EditorGUILayout.BeginToggleGroup("Check continuously", continuousProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousEvents"));
        EditorGUILayout.EndToggleGroup();

        continuouNegativesProp.boolValue = EditorGUILayout.BeginToggleGroup("Check continuously (negative)", continuouNegativesProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousNegativeEvents"));
        EditorGUILayout.EndToggleGroup();

        fireOnceProp.boolValue = EditorGUILayout.BeginToggleGroup("Check first change", fireOnceProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceEvents"));
        EditorGUILayout.EndToggleGroup();

        fireOnceNegativeProp.boolValue = EditorGUILayout.BeginToggleGroup("Check first change (negative)", fireOnceNegativeProp.boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceNegativeEvents"));
        EditorGUILayout.EndToggleGroup();
    }
}
