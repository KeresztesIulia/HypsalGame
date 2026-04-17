using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(abstract_ConditionalRunner))]  
public class CI_abstract_ConditionalRunner : Editor
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
        var onAssociationProp = serializedObject.FindProperty("_checkAssociation");
        var onAssociationNegativeProp = serializedObject.FindProperty("_checkAssociation_negative");

        atStartProp.boolValue = EditorGUILayout.Toggle("Check at start", atStartProp.boolValue);
        if (atStartProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartEvents"));

        EditorGUILayout.Space(2);

        atStartNegativeProp.boolValue = EditorGUILayout.Toggle("Check at start (negative)", atStartNegativeProp.boolValue);
        if (atStartNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_AtStartNegativeEvents"));

        EditorGUILayout.Space(2);

        changeProp.boolValue = EditorGUILayout.Toggle("Check change", changeProp.boolValue);
        if (changeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeEvents"));

        EditorGUILayout.Space(2);

        changeNegativeProp.boolValue = EditorGUILayout.Toggle("Check change (negative)", changeNegativeProp.boolValue);
        if (changeNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnChangeNegativeEvents"));

        EditorGUILayout.Space(2);

        continuousProp.boolValue = EditorGUILayout.Toggle("Check continuously", continuousProp.boolValue);
        if (continuousProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousEvents"));

        EditorGUILayout.Space(2);

        continuouNegativesProp.boolValue = EditorGUILayout.Toggle("Check continuously (negative)", continuouNegativesProp.boolValue);
        if (continuouNegativesProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_ContinuousNegativeEvents"));

        EditorGUILayout.Space(2);

        fireOnceProp.boolValue = EditorGUILayout.Toggle("Check first change", fireOnceProp.boolValue);
        if (fireOnceProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceEvents"));

        EditorGUILayout.Space(2);

        fireOnceNegativeProp.boolValue = EditorGUILayout.Toggle("Check first change (negative)", fireOnceNegativeProp.boolValue);
        if (fireOnceNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_FireOnceNegativeEvents"));

        EditorGUILayout.Space(2);

        onAssociationProp.boolValue = EditorGUILayout.Toggle("Check labeling result", onAssociationProp.boolValue);
        if (onAssociationProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnAssociationEvents"));

        EditorGUILayout.Space(2);

        onAssociationNegativeProp.boolValue = EditorGUILayout.Toggle("Check labeling result (negative)", onAssociationNegativeProp.boolValue);
        if (onAssociationNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnAssociationNegativeEvents"));
    }
}
