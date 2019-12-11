using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Method))]
public class MethodPropertyDrawer : PropertyDrawer
{

    List<MethodInfo> methods;
    GUIContent[] names;

    static readonly GUIContent noMethods = new GUIContent("--No methods--");
    static readonly GUIContent toggleBaseClass = new GUIContent("", "Show methods from base classes");

    SerializedProperty property;
    GUIContent label;

    object parent;
    Object target;
    Rect contentRect;
    Rect toggleRect;

    Method Value
    {
        get => (Method)property.objectReferenceValue;
        set => property.objectReferenceValue = value;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        this.property = property;
        this.label = label;
        parent = property.GetParent();

        target = property.serializedObject.targetObject;
        contentRect = new Rect(position.x, position.y, position.width - 22, position.height);
        toggleRect = new Rect(position.xMax - 16 - (EditorGUI.indentLevel * 15), position.y, 22, position.height);

        BaseClassToggle();
        MethodPicker();

    }
     
    void BaseClassToggle()
    {

        EditorGUI.BeginChangeCheck();
        property.isExpanded = EditorGUI.Toggle(toggleRect, property.isExpanded);
        EditorGUI.LabelField(toggleRect, toggleBaseClass);

        if (EditorGUI.EndChangeCheck() || methods == null || methods.Count == 0)
            GetMethods(target, property.isExpanded);

    }

    void MethodPicker()
    {
         
        if (methods == null || methods?.Count == 0)
        {
            EditorGUI.LabelField(contentRect, label, noMethods);
            return;
        }

        EditorGUI.BeginChangeCheck();

        var i = methods.IndexOf(Value);
        i = EditorGUI.Popup(contentRect, label, i, names);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Changed method name");
            Value = Method.CreateInstance(methods[i], target);
        }
         
    }
     
    void GetMethods(object target, bool includeBase)
    {

        var t = target.GetType();
        methods = t.GetMethods().
            Where(m => 
                 !m.Name.StartsWith("set_") && !m.Name.StartsWith("get_") && //Don't include setters and getters for properties
                 (includeBase ? true : m.DeclaringType == t)). //Only show base class methods if toggle is checked
            ToList();

        methods.Insert(0, null);

        names = methods.Select(m => new GUIContent(GetName(m), GetName(m))).ToArray();

    }

    string GetName(MethodInfo method)
    {
        if (method == null)
            return "None";
        return method.Name + "(" + string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) + ")";
    }

}
#endif
