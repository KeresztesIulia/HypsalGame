using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_LabelReplica))]
public class CI_LabelReplica : Editor
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

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_replicaParents"));

        EditorGUILayout.Space();
        var replicatedLabelsProp = serializedObject.FindProperty("_replicatedLabels");
        EditorGUILayout.LabelField("Replicated labels", EditorStyles.boldLabel);

        for (int i = 0; i < replicatedLabelsProp.arraySize; i++)
        {
            var replicatedLabelProp = replicatedLabelsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            replicatedLabelProp.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, replicatedLabelProp.stringValue);

            if (GUILayout.Button("-"))
            {
                replicatedLabelsProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+"))
        {
            replicatedLabelsProp.arraySize++;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_triggerAreas"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_triggerOnce"));

        serializedObject.ApplyModifiedProperties();
    }
}
