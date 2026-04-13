using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CustomEditor(typeof(script_LabelingResponseHandler))]
public class CI_LabelingResponseHandler : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(this), GetType(), false);
        }

        var labelListProp = serializedObject.FindProperty("_labelList");
        EditorGUILayout.PropertyField(labelListProp);

        var labelList = labelListProp.objectReferenceValue as script_so_LabelList;
        if (labelList is null) return;

        EditorGUILayout.Space(10);

        var firstLabelsProp = serializedObject.FindProperty("_firstLabels");
        var secondLabelsProp = serializedObject.FindProperty("_secondLabels");
        var responsesProp = serializedObject.FindProperty("_responses");

        GUIStyle style = new(EditorStyles.boldLabel);
        style.fontSize += 3;

        EditorGUILayout.LabelField("Special responses", style);
        for (int i = 0; i < firstLabelsProp.arraySize; i++)
        {
            var firstLabel = firstLabelsProp.GetArrayElementAtIndex(i);
            var secondLabel = secondLabelsProp.GetArrayElementAtIndex(i);
            var response = responsesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            firstLabel.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, firstLabel.stringValue);
            secondLabel.stringValue = CI_CustomInspectorUtilities.LabelNameField(labelList, secondLabel.stringValue);
            EditorGUILayout.EndHorizontal();

            EditorStyles.textField.wordWrap = true;
            response.stringValue = EditorGUILayout.TextArea(response.stringValue);
            
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(100), GUILayout.ExpandHeight(true)))
            {
                firstLabelsProp.DeleteArrayElementAtIndex(i);
                secondLabelsProp.DeleteArrayElementAtIndex(i);
                responsesProp.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);
        }
        if (GUILayout.Button("+"))
        {
            firstLabelsProp.arraySize++;
            secondLabelsProp.arraySize++;
            responsesProp.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();

    }
}
