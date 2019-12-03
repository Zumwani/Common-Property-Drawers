using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Displays a label with the content of this variable.</summary>
public class LabelAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer<LabelAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(fieldInfo.GetValue(property.serializedObject.targetObject)?.ToString()));
    }

}
#endif
