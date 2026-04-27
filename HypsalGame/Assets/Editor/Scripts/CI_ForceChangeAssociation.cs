using UnityEditor;

[CustomEditor(typeof(script_ForceChangeAssociation))]
public class CI_ForceChangeAssociation : Editor
{
    public override void OnInspectorGUI()
    {
        CI_CustomInspectorUtilities.ScriptReferences(this);

        var labelListProp = serializedObject.FindProperty("_labelList");
        EditorGUILayout.PropertyField(labelListProp);

        var labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList == null) return;


        var labelProp = serializedObject.FindProperty("_label");
        labelProp.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, labelProp.stringValue, "Label");

        if (string.IsNullOrEmpty(labelProp.stringValue))
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }


        var associatedLabelProp = serializedObject.FindProperty("_associatedLabel");
        associatedLabelProp.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, associatedLabelProp.stringValue, "Associated label");

        if (!string.IsNullOrEmpty(associatedLabelProp.stringValue))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_fireAssociationEvents"));
        }

        var keepOriginalProp = serializedObject.FindProperty("_keepOriginalModel");
        EditorGUILayout.PropertyField(keepOriginalProp);

        if (!keepOriginalProp.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("_newModel"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_addOnce"));

        serializedObject.ApplyModifiedProperties();
    }
}
