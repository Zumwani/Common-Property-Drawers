using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Automatically finds the <see cref="Component"/> on this game object.</summary>
public class AutoAttribute : PropertyAttribute
{ }

/// <summary>Automatically finds the <see cref="Component"/> on this game object, and displays a warning if none is found.</summary>
public class RequiredAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RequiredAttribute))]
[CustomPropertyDrawer(typeof(AutoAttribute))]
public class AutoDrawer : PropertyDrawer
{

    static readonly GUIStyle redStyle = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } };
    static readonly GUIContent missing = new GUIContent("Missing:");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.ObjectReference)
            return;

        bool hasValue = property.objectReferenceValue;
        var isTypeMismatch = hasValue && fieldInfo.FieldType != property.objectReferenceValue.GetType();

        if (hasValue && !isTypeMismatch)
            return;

        property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponent(fieldInfo.FieldType);

        if (attribute is RequiredAttribute && !property.objectReferenceValue)
            EditorGUI.LabelField(position, missing, new GUIContent(fieldInfo.FieldType.Name), redStyle);

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (attribute is RequiredAttribute && !property.objectReferenceValue)
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }

}
#endif
