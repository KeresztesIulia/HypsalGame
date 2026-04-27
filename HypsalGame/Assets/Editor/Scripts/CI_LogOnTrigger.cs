using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_LogOnTrigger))]
public class CI_LogOnTrigger : Editor
{
    public override void OnInspectorGUI()
    {
        CI_CustomInspectorUtilities.ScriptReferences(this);

        MakeList();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_triggerOnce"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_popUpLog"));

        serializedObject.ApplyModifiedProperties();
    }

    void MakeList()
    {
        var textToLogProp = serializedObject.FindProperty("_textsToLog");
        var logTypesProp = serializedObject.FindProperty("_logTypes");

        EditorGUILayout.LabelField("Texts to log", EditorStyles.boldLabel);

        for (int i = 0; i < textToLogProp.arraySize; i++)
        {
            var textProp = textToLogProp.GetArrayElementAtIndex(i);
            var logTypeProp = logTypesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            textProp.stringValue = EditorGUILayout.TextField(textProp.stringValue);
            logTypeProp.enumValueIndex = EditorGUILayout.Popup("",
                                                            logTypeProp.enumValueIndex,
                                                            logTypeProp.enumDisplayNames);

            if (GUILayout.Button("-"))
            {
                textToLogProp.DeleteArrayElementAtIndex(i);
                logTypesProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+"))
        {
            textToLogProp.arraySize++;
            logTypesProp.arraySize++;
        }
    }
}
