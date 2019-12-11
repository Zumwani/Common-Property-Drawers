using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Method : ScriptableObject, ISerializationCallbackReceiver
{
    
    public Method Set(MethodInfo method, Object target = null)
    {
        this.methodInfo = method;
        this.target = target;
        return this;
    }

    public static Method CreateInstance(MethodInfo method, Object target = null)
    {
        return CreateInstance<Method>().Set(method, target);
    }

    public MethodInfo methodInfo;
    public SerializableType type;
    public string methodName;
    public List<SerializableType> parameters = null;
    public int flags = 0;

    /// <summary>Target that is set when using from inspector.</summary>
    public Object target;

    public void Invoke(object target, params object[] param)
    {
        methodInfo.Invoke(target, param);
    }

    /// <summary>Invoke using target received from setting value in inspector.</summary>
    public void Invoke(params object[] param)
    {
        Invoke(target, param);
    }

    public void OnBeforeSerialize()
    {

        if (methodInfo == null)
            return;

        type = new SerializableType(methodInfo.DeclaringType);
        methodName = methodInfo.Name;

        flags |= methodInfo.IsPrivate ? (int)BindingFlags.NonPublic : (int)BindingFlags.Public;
        flags |= methodInfo.IsStatic  ? (int)BindingFlags.Static    : (int)BindingFlags.Instance;

        var p = methodInfo.GetParameters();
        if (p != null && p.Length > 0)
        {

            parameters = new List<SerializableType>(p.Length);
            for (int i = 0; i < p.Length; i++)
                parameters.Add(new SerializableType(p[i].ParameterType));

        }
        else
            parameters = null;

    }

    public void OnAfterDeserialize()
    {

        if (type == null || string.IsNullOrEmpty(methodName))
            return;

        var t = type.type;
        Type[] param = null;

        if (parameters != null && parameters.Count > 0)
        {
            param = new System.Type[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
                param[i] = parameters[i].type;
        }

        if (param == null)
            methodInfo = t.GetMethod(methodName, (BindingFlags)flags);
        else
            methodInfo = t.GetMethod(methodName, (BindingFlags)flags, null, param, null);

    }
    
    public static implicit operator Method(MethodInfo method)
    {
        return CreateInstance(method);
    }

    public static implicit operator MethodInfo(Method method)
    {
        if (method)
            return method.methodInfo;
        return default;
    }

}
