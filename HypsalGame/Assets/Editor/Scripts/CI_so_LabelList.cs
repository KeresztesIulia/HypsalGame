using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(script_so_LabelList))]  
public class CI_so_LabelList : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset \"saved\" data"))
        {
            (target as script_so_LabelList).Reset();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
