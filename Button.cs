using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Displays a button in the inspector.</summary>
public class ButtonAttribute : PropertyAttribute
{

    public string function;

    /// <summary>Sets the bool to true on the frame the button is pressed.</summary>
    public ButtonAttribute()
    { }

    /// <summary>Calls the function when the button is pressed.</summary>
    public ButtonAttribute(string function)
    { this.function = function; }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : PropertyDrawer<ButtonAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.Boolean)
            return;

        property.boolValue = GUI.Button(position, label);
        if (property.boolValue)
            property.serializedObject.targetObject.GetType().GetMethod(attribute.function)?.Invoke(property.serializedObject.targetObject, System.Array.Empty<object>());

    }

}
#endif
