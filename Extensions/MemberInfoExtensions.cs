using System.Reflection;

public static class MemberInfoExtensions
{

    public static object GetValue(this MemberInfo member, object instance, params object[] param)
    {
        switch (member?.MemberType)
        {
            case MemberTypes.Property:
                return ((PropertyInfo)member).GetValue(instance, param);
            case MemberTypes.Field:
                return ((FieldInfo)member).GetValue(instance);
            case MemberTypes.Method:
                return ((MethodInfo)member).Invoke(instance, param);
            default:
                return null;
        }
    }

    public static void SetValue(this MemberInfo member, object instance, object value)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Property:
                ((PropertyInfo)member).SetValue(instance, value); break;
            case MemberTypes.Field:
                ((FieldInfo)member).SetValue(instance, value); break;
        }
    }

    public static System.Reflection.MethodInfo GetSetter(this MemberInfo member)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Property:
                return ((PropertyInfo)member).GetSetter();
            case MemberTypes.Field:
                return null;
            case MemberTypes.Method:
                return (System.Reflection.MethodInfo)member;
            default:
                return null;
        }
    }

}
