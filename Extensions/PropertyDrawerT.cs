#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public abstract class PropertyDrawer<T> : PropertyDrawer where T : PropertyAttribute
{

    public new T attribute => (T)base.attribute;

}
#endif
