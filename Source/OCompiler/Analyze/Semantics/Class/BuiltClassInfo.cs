using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OCompiler.Analyze.Semantics.Class;

internal class BuiltClassInfo : ClassInfo
{
    public override Type Class { get; }
    public override Type? BaseClass { get; }
    public List<MethodInfo> Methods { get; }
    public List<FieldInfo> Fields { get; }
    public List<ConstructorInfo> Constructors { get; }
    public static Dictionary<string, ClassInfo> StandardClasses { get; private set; }

    public BuiltClassInfo(Type builtClassType)
    {
        Class = builtClassType;
        BaseClass = builtClassType.BaseType;
        Name = builtClassType.Name;
        Methods = builtClassType.GetRuntimeMethods().ToList();
        Fields = builtClassType.GetRuntimeFields().ToList();
        Constructors = builtClassType.GetConstructors().ToList();
    }

    static BuiltClassInfo()
    {
        StandardClasses = LoadStandardClasses();
    }

    private static Dictionary<string, ClassInfo> LoadStandardClasses(string @namespace = "OCompiler.StandardLibrary.Type")
    {
        var asm = Assembly.GetExecutingAssembly();
        return new Dictionary<string, ClassInfo>(
            asm.GetTypes().Where(
                type => (type.IsClass || type.IsValueType) &&
                type.Namespace != null &&
                type.Namespace.StartsWith(@namespace)
            ).Select(type => new KeyValuePair<string, ClassInfo>(type.Name, new BuiltClassInfo(type)))
        );
    }

    public override string? GetMethodReturnType(string name, List<string> argumentTypes)
    {
        var method = Methods.Where(
            m => m.Name == name &&
            m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(argumentTypes)
        ).FirstOrDefault();

        return method?.ReturnType.Name;
    }

    public override string? GetFieldType(string name)
    {
        var field = Fields.Where(f => f.Name == name).FirstOrDefault();
        return field?.FieldType.Name;
    }

    public override bool HasField(string name)
    {
        return GetFieldType(name) != null;
    }

    public override bool HasConstructor(List<string> argumentTypes)
    {
        var candidates = Constructors.Where(
            c => c.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(argumentTypes)
        ).ToList();
        if (candidates.Count > 1)
        {
            throw new Exception($"More than one constructor matches signature {Name}({string.Join(",", argumentTypes)})");
        }

        return candidates.Count == 1;
    }

    public override string ToString()
    {
        StringBuilder @string = new();
        @string.Append("Standard library class ");
        @string.Append(Name);
        if (BaseClass != null && BaseClass != typeof(object))
        {
            @string.Append(" extends ");
            @string.Append(BaseClass.Name);
        }
        return @string.ToString();
    }
}
