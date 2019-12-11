using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FolderPickerAttribute : PropertyAttribute
{ }

public class FilePickerAttribute : FolderPickerAttribute
{
    public string extension;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(FilePickerAttribute))]
[CustomPropertyDrawer(typeof(FolderPickerAttribute))]
public class FilePickerPropertyDrawer : PropertyDrawer<FolderPickerAttribute>
{

    static readonly GUIContent file = new GUIContent("...", "Open file...");
    static readonly GUIContent folder = new GUIContent("...", "Open folder...");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (position.width == 1) //Why is position (0, 0, 1, 1) every other frame?
            return;

        if (property.propertyType != SerializedPropertyType.String)
            return;

        bool isFile = attribute is FilePickerAttribute;

        var labelRect = new Rect(position.x, position.y, position.width - 22, position.height);
        var contentRect = new Rect(position.x, position.y + (labelRect.height / 2), position.width - 22, position.height);
        var buttonRect = new Rect(position.xMax - 22, contentRect.y + 12, 19, 16);

        var path = ShortenPath(property.stringValue, contentRect.width - 42, isFile);

        EditorGUI.LabelField(labelRect, label);
        EditorGUI.LabelField(contentRect, new GUIContent(path, property.stringValue));
        if (GUI.Button(buttonRect, isFile ? file : folder))
        {
            path = PickPath(property.stringValue, isFile);
            if (!string.IsNullOrEmpty(path))
                property.stringValue = path;
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 2;
    }

    string PickPath(string path, bool isFile)
    {

        if (string.IsNullOrWhiteSpace(path))
            path = Application.dataPath;

        var name = $"Pick {(isFile ? "file" : "folder")}...";
        var ext = (attribute as FilePickerAttribute)?.extension;

        if (isFile)
            path = EditorUtility.OpenFilePanel(name, path, ext);
        else
            path = EditorUtility.OpenFolderPanel(name, path, "");
        
        if (!string.IsNullOrWhiteSpace(path))
        {
            if (path.StartsWith(Application.dataPath))
                path = path.Remove(0, Application.dataPath.Length);
            return path;
        }

        return "";

    }

    public static string ShortenPath(string path, float maxWidth, bool isFile)
    {

        if (path.Length < 6)
            return path;

        var one = EditorStyles.label.CalcSize(new GUIContent(" ")).x;
        var full = one * path.Length;

        if (full + 42 < maxWidth)
            return path;
            
        var folder = isFile ? GetParent(path).Name : GetFileNameWithoutExtension(path);
        var count = Mathf.CeilToInt(maxWidth / one) - (folder.Length + ".../".Length + Mathf.CeilToInt((maxWidth / full) * 4f));

        if (count < 1)
            return ".../" + folder;

        var s = path.Remove(count);

        return s + ".../" + folder;

    }

}

#endif