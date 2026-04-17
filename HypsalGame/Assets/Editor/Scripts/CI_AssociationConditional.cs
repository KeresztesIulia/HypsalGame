using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_AssociationConditional))]
public class CI_AssociationConditional : CI_abstract_ConditionalRunner
{
    script_so_LabelList labelList;

    protected override bool InitialSetup()
    {
        var labelListProp = serializedObject.FindProperty("_labelList");
        EditorGUILayout.PropertyField(labelListProp);

        labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList == null) return false;
        return true;
    }

    protected override void SetUpConditionField()
    {
        var conditionLabel1Prop = serializedObject.FindProperty("_conditionLabelName1");
        var conditionLabel2Prop = serializedObject.FindProperty("_conditionLabelName2");

        EditorGUILayout.BeginHorizontal();

        conditionLabel1Prop.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, conditionLabel1Prop.stringValue);
        EditorGUILayout.LabelField(" = ", GUILayout.Width(15));
        conditionLabel2Prop.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, conditionLabel2Prop.stringValue);

        EditorGUILayout.EndHorizontal();
    }

    protected override void SetUpExtraEvents()
    {
        var onAssociationProp = serializedObject.FindProperty("_checkAssociation");
        var onAssociationNegativeProp = serializedObject.FindProperty("_checkAssociation_negative");

        onAssociationProp.boolValue = EditorGUILayout.Toggle("Check labeling result", onAssociationProp.boolValue);
        if (onAssociationProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnAssociationEvents"));

        EditorGUILayout.Space();

        onAssociationNegativeProp.boolValue = EditorGUILayout.Toggle("Check labeling result (negative)", onAssociationNegativeProp.boolValue);
        if (onAssociationNegativeProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_OnAssociationNegativeEvents"));
    }
}
