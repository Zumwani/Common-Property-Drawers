using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Hides the variable in the inspector.</summary>
public class HiddenAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HiddenAttribute))]
public class HiddenPropertyDrawer : PropertyDrawer<HiddenAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

}
#endif
