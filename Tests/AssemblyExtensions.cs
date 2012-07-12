using System;
using System.Reflection;

public static class AssemblyExtensions
{

    public static dynamic GetInstance(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        //dynamic instance = FormatterServices.GetUninitializedObject(type);
        return Activator.CreateInstance(type);
    }
    public static dynamic GetInstance<T>(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className + "`1", true);
        return Activator.CreateInstance(type.MakeGenericType(typeof(T)));
    }
}