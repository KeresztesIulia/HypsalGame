using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_LabelRepresentative)), CanEditMultipleObjects]
public class CI_LabelRepresentative : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();


        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(this), GetType(), false);
        }

        // look for LabelList
        // if not found, return
        var labelListProp = serializedObject.FindProperty("_partOfList");
        EditorGUILayout.PropertyField(labelListProp);

        var labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList == null) return;


        // create dropdown for _representedLabel (through Utilities)
        var representedLabelProp = serializedObject.FindProperty("_representedLabelName");
        representedLabelProp.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, representedLabelProp.stringValue, "Represented label");

        // create List view with dropdowns for _possibleAssociations
        var possibleAssociationsProp = serializedObject.FindProperty("_possibleAssociations");

        EditorGUILayout.Space(10);

        GUIStyle style = new(EditorStyles.boldLabel);
        style.fontSize += 3;

        EditorGUILayout.LabelField("Possible associations", style);
        for (int i = 0; i < possibleAssociationsProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            var associationProp = possibleAssociationsProp.GetArrayElementAtIndex(i);
            
            associationProp.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, associationProp.stringValue);

            if (GUILayout.Button("-")) possibleAssociationsProp.DeleteArrayElementAtIndex(i);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+")) possibleAssociationsProp.arraySize++;

        if (possibleAssociationsProp.arraySize > 0)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_prompt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_relabelable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiInfo"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
