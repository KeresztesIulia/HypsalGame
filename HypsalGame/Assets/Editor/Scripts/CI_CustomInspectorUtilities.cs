using UnityEditor;
using UnityEngine;

public static class CI_CustomInspectorUtilities
{
    public static void ScriptReferences(Editor customInspector)
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)customInspector.target), customInspector.GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(customInspector), customInspector.GetType(), false);
        }
    }

    public static Label LabelField(script_so_LabelList labelList, Label currentValue, string CILabel = "")
    {
        var names = labelList.LabelStrings;
        names.Insert(0, "NONE");

        var currentNameIndex = labelList.GetLabelIndex(currentValue);

        EditorGUILayout.BeginHorizontal();
        if (!string.IsNullOrEmpty(CILabel)) EditorGUILayout.LabelField(CILabel);
        int newIndex = EditorGUILayout.Popup(currentNameIndex + 1, names.ToArray()) - 1;
        EditorGUILayout.EndHorizontal();

        // Return The Label
        // or null if invalidated
        if (currentNameIndex != newIndex)
        {
            return labelList.GetLabel(newIndex);
        }
        else
        {
            return currentValue;
        }
    }

    public static string LabelNameField(script_so_LabelList labelList, string currentValue, string CILabel = "")
    {
        var names = labelList.LabelStrings;
        names.Insert(0, "NONE");

        var currentNameIndex = labelList.GetLabelIndex(currentValue);

        EditorGUILayout.BeginHorizontal();
        if (!string.IsNullOrEmpty(CILabel)) EditorGUILayout.LabelField(CILabel);
        int newIndex = EditorGUILayout.Popup(currentNameIndex + 1, names.ToArray()) - 1;
        EditorGUILayout.EndHorizontal();

        if (newIndex == -1) return null;

        return names[newIndex + 1];
    }

    public static void LabelNameField(script_so_LabelList labelList, SerializedProperty property, string CILabel = "")
    {
        if (labelList == null) return;

        property.stringValue = LabelNameField(labelList, property.stringValue, CILabel);
    }

    
}
