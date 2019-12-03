using UnityEngine;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Shows the variable if an condition is met.</summary>
public class ShowIfAttribute : PropertyAttribute
{

    public string propertyName;
    public object value;
    public bool useValue;
    public bool invert;

    public ShowIfAttribute(string name)
    {
        propertyName = name;
        useValue = false;
    }

    public ShowIfAttribute(string name, object value = null)
    {
        propertyName = name;
        this.value = value;
        useValue = true;
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute), false)]
public class ShowIfPropertyDrawer : PropertyDrawer<ShowIfAttribute>
{

    const BindingFlags bindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Static | BindingFlags.Instance |
        BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

    bool isTrue = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var obj = property.GetParent();
        if (obj == null)
            return;

        isTrue = Eval(obj, attribute);

        if (isTrue)
            EditorGUI.PropertyField(position, property);

    }

    public static bool Eval(object obj, ShowIfAttribute attribute)
    {

        bool isTrue = false;
        var member = obj.GetType().GetMember(attribute.propertyName, bindingFlags).FirstOrDefault();

        if (member == null)
            isTrue = true;
        else if (member.GetValue(obj) is object value)
            if (attribute.useValue)
                isTrue = Equals(value, attribute.value);
            else if (value is bool)
                isTrue = (bool)value == true;
            else if (value is Object)
                isTrue = (Object)value;
            else if (value != null)
                isTrue = true;

        if (attribute.invert)
            isTrue = !isTrue;

        return isTrue;

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (isTrue)
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }

}
#endif
