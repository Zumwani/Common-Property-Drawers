# Common-Property-Drawers

This package provides a few commonly useful property drawers:
* Auto - Automatically gets a component on the game object, parent or children.
* Required - Same as Auto but displays a warning when component could not be found.
* Button - Displays a button instead of a toggle for a bool, which could either be checked in OnValidate or be bound to a method.
* Label - Displays the content of the variable in text.
* ShowIf - Only shows variable if condition is met.
* Method - This library provides a serializable method info and in the inspector allows you to pick methods on the current script. 
* FilePicker / FolderPicker - Shows a button in the inspector which opens a file / folder picker.

# Usage:
Most are implemented as property attributes with one exception being Method.

```
public class Test : MonoBehaviour
{

  [FolderPicker]
  public string path;

  public Method method;

  [Button(nameof(method))]
  public bool invokeMethodDirectly;

  [Button(nameof(InvokeMethodAfterDebugMessage))]
  public bool invokeMethodWithDebug;

  void InvokeMethodAfterDebugMessage()
  {
    Debug.Log("Invoking method...");
    method.Invoke();
  }

}
```

# Installation
Installation can be done through the Unity Package Manager.
In the package manager window press the '+' button and choose 'add package from git url' and paste the path to the git repo:

https://github.com/Zumwani/Common-Property-Drawers.git
