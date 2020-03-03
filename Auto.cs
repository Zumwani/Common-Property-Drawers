using UnityEngine;
using static AutoAttribute;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Automatically finds the <see cref="Component"/> on this game object.</summary>
public class AutoAttribute : PropertyAttribute
{

    public enum Direction
    {
        Self, Children, Parents
    }

    public Direction direction;
    public bool isRequired;

    public AutoAttribute()
    { }

    public AutoAttribute(Direction direction)
    {
        this.direction = direction;
    }

    public AutoAttribute(Direction direction, bool isRequired)
    {
        this.direction = direction;
        this.isRequired = isRequired;
    }

}

/// <summary>Automatically finds the <see cref="Component"/> on this game object, and displays a warning if none is found.</summary>
public class RequiredAttribute : AutoAttribute
{

    public RequiredAttribute() : base(Direction.Self, true)
    { }

    public RequiredAttribute(Direction direction) : base(direction, true)
    { }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AutoAttribute))]
[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class AutoDrawer : PropertyDrawer<AutoAttribute>
{

    static readonly GUIStyle redStyle = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } };
    static readonly GUIContent missing = new GUIContent("Missing:");

    bool isFirstTime = true;
    bool IsUndo;
    void OnUndo() => IsUndo = true;
    public AutoDrawer() => Undo.undoRedoPerformed += OnUndo;
    ~AutoDrawer()       => Undo.undoRedoPerformed -= OnUndo;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var force = isFirstTime || IsUndo;
        isFirstTime = false;

        if (property.propertyType != SerializedPropertyType.ObjectReference && !force)
            return;

        bool hasValue = property.objectReferenceValue;
        var isTypeMismatch = hasValue && fieldInfo.FieldType != property.objectReferenceValue.GetType();

        if (!force && hasValue && !isTypeMismatch)
            return;

        var target = ((Component)property.serializedObject.targetObject);
        property.objectReferenceValue = GetComponent(target, fieldInfo.FieldType);

        if (attribute.isRequired && !property.objectReferenceValue)
            EditorGUI.LabelField(position, missing, new GUIContent(fieldInfo.FieldType.Name), redStyle);

    }

    Component GetComponent(Component target, Type type)
    {
        switch (attribute.direction)
        {
            case Direction.Self:
                return target.GetComponent(type);
            case Direction.Children:
                return target.GetComponentInChildren(type);
            case Direction.Parents:
                return target.GetComponentInParent(type);
            default:
                return default;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (attribute.isRequired && !property.objectReferenceValue)
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }
#endif

}
