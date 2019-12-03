#if UNITY_EDITOR
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

}
#endif
