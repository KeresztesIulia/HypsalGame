using UnityEditor;

[CustomEditor(typeof(script_SpecificObjectLabelingConditional))]
public class CI_SpecificObjectLabelingCondition : CI_abstract_ConditionalRunner
{
    protected override void SetUpConditionField()
    {
        var labelableObjectsProp = serializedObject.FindProperty("_labelableObjects");
        EditorGUILayout.PropertyField(labelableObjectsProp);

        int maxValue = labelableObjectsProp.arraySize;
        var numberOfObjectsToLabelProp = serializedObject.FindProperty("_numberOfObjectsToLabel");
        numberOfObjectsToLabelProp.intValue = (int)EditorGUILayout.Slider(numberOfObjectsToLabelProp.intValue, 0, maxValue);
    }
}
