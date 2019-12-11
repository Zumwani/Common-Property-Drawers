using UnityEngine;
using System.Linq;
using System.Reflection;

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

    const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.Boolean)
            return;

        property.boolValue = GUI.Button(position, label);
        if (property.boolValue)
        {

            var target = property.serializedObject.targetObject;
            var empty = System.Array.Empty<object>();

            var method = target.GetType().GetMethod(attribute.function, BindingFlags.InvokeMethod | flags);
            method?.Invoke(target, empty);

            var field = target.GetType().GetField(attribute.function, BindingFlags.GetField | flags);
            if (field.GetValue(target, empty) is object m)
                if (m is MethodInfo mi)
                    mi.Invoke(target, empty);
                else if (m is Method mr)
                    mr.Invoke();

        }

    }

}
#endif
