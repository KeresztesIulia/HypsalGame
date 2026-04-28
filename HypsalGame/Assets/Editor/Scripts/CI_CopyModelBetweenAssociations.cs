using UnityEditor;

[CustomEditor(typeof(script_CopyModelBetweenAssociations))]
public class CI_CopyModelBetweenAssociations : Editor
{
    public override void OnInspectorGUI()
    {
        CI_CustomInspectorUtilities.ScriptReferences(this);

        var labelListProp = serializedObject.FindProperty("_labelList");
        EditorGUILayout.PropertyField(labelListProp);

        var labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList == null) return;

        CI_CustomInspectorUtilities.LabelNameField(labelList, serializedObject.FindProperty("_sourceLabel"), "Source label");
        CI_CustomInspectorUtilities.LabelNameField(labelList, serializedObject.FindProperty("_destinationLabel"), "Destination label");

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_keepOriginalOnEmpty"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_copyOnce"));

        serializedObject.ApplyModifiedProperties();
    }
}
