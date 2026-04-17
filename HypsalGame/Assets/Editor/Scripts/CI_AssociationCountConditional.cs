using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_AssociationCountConditional))]
public class CI_AssociationCountConditional : CI_abstract_ConditionalRunner
{
    protected override void SetUpConditionField()
    {
        SerializedProperty fixedValueProp = serializedObject.FindProperty("_fixedValue");
        EditorGUILayout.PropertyField(fixedValueProp);

        if (fixedValueProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetCountRange"));
        else EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetCount"));
    }
}
