#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class PropertyDrawerExtensions
{

    public static object GetParent(this SerializedProperty prop)
    {

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');

        foreach (var element in elements.Take(elements.Length - 1))
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            }
            else
                obj = GetValue(obj, element);

        return obj;

    }

    public static object GetValue(this object source, string name)
    {

        if (source == null)
            return null;

        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;
            return p.GetValue(source, null);
        }

        return f.GetValue(source);

    }

    public static object GetValue(object source, string name, int index)
    {

        var enumerable = GetValue(source, name) as IEnumerable;
        var enm = enumerable.GetEnumerator();

        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;

    }

    public static GUIStyle Color(this GUIStyle style, Color color)
    {
        style = new GUIStyle(style);
        style.normal.textColor = color;
        return style;
    }

    public static void RepaintInspector(this SerializedObject BaseObject)
    {
        var inspector = ActiveEditorTracker.sharedTracker.activeEditors.FirstOrDefault(i => i.serializedObject == BaseObject);
        inspector.Repaint();
    }

    public static void SetValueDirect(this SerializedProperty property, object value)
    {

        if (property == null)
            throw new System.NullReferenceException("SerializedProperty is null");

        object obj = property.serializedObject.targetObject;
        string propertyPath = property.propertyPath;
        var flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var paths = propertyPath.Split('.');
        FieldInfo field = null;

        for (int i = 0; i < paths.Length; i++)
        {
            var path = paths[i];
            if (obj == null)
                throw new System.NullReferenceException("Can't set a value on a null instance");

            var type = obj.GetType();
            if (path == "Array")
            {
                path = paths[++i];
                var iter = (obj as IEnumerable);
                if (iter == null)
                    //Property path thinks this property was an enumerable, but isn't. property path can't be parsed
                    throw new System.ArgumentException("SerializedProperty.PropertyPath [" + propertyPath + "] thinks that [" + paths[i - 2] + "] is Enumerable.");

                var sind = path.Split('[', ']');
                int index = -1;

                if (sind == null || sind.Length < 2)
                    // the array string index is malformed. the property path can't be parsed
                    throw new System.FormatException("PropertyPath [" + propertyPath + "] is malformed");

                if (!int.TryParse(sind[1], out index))
                    //the array string index in the property path couldn't be parsed,
                    throw new System.FormatException("PropertyPath [" + propertyPath + "] is malformed");

                obj = iter.Cast<object>().ElementAtOrDefault(index);
                continue;
            }

            field = type.GetField(path, flag);
            if (field == null)
                //field wasn't found
                throw new System.MissingFieldException("The field [" + path + "] in [" + propertyPath + "] could not be found");

            if (i < paths.Length - 1)
                obj = field.GetValue(obj);

        }

        var valueType = value.GetType();
        if (!valueType.Is(field.FieldType))
            // can't set value into field, types are incompatible
            throw new System.InvalidCastException("Cannot cast [" + valueType + "] into Field type [" + field.FieldType + "]");

        field.SetValue(obj, value);

    }

    public static bool Is(this Type type, Type baseType)
    {

        if (type == null) return false;
        if (baseType == null) return false;

        return baseType.IsAssignableFrom(type);

    }

    public static bool Is<T>(this Type type)
    {

        if (type == null) return false;
        Type baseType = typeof(T);

        return baseType.IsAssignableFrom(type);
    }

}
#endif
