using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_Nameable))]
public class CI_Nameable : Editor
{
    public override void OnInspectorGUI()
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.ObjectField("Custom Inspector", MonoScript.FromScriptableObject(this), GetType(), false);
        }

        var nameListProp = serializedObject.FindProperty("_nameList");
        EditorGUILayout.PropertyField(nameListProp);

        var representedWordProp = serializedObject.FindProperty("_representedWord");

        var nameList = nameListProp.objectReferenceValue as script_so_NameList;
        if (nameList == null) return;

        var names = nameList.wordStrings.ToList();
        names.Insert(0, "INVALID");

        var currentNameIndex = nameList.GetWordIndex(representedWordProp.boxedValue as LabelableWord);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Represented word");
        int newIndex = EditorGUILayout.Popup(currentNameIndex + 1, names.ToArray()) - 1;
        EditorGUILayout.EndHorizontal();

        if (currentNameIndex != newIndex)
        {
            representedWordProp.boxedValue = nameList.GetWord(newIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
