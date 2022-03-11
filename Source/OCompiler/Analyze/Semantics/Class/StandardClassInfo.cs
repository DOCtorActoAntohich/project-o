using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OCompiler.Analyze.Semantics.Class;

internal class StandardClassInfo : ClassInfo
{
    public override Type Class { get; }
    public override Type? BaseClass { get; }
    public List<MethodInfo> Methods { get; }
    public List<FieldInfo> Fields { get; }
    public List<ConstructorInfo> Constructors { get; }
    public static Dictionary<string, ClassInfo> StandardClasses { get; private set; }

    public StandardClassInfo(Type standardClassType)
    {
        Class = standardClassType;
        BaseClass = standardClassType.BaseType;
        Name = standardClassType.Name;
        Methods = standardClassType.GetRuntimeMethods().ToList();
        Fields = standardClassType.GetRuntimeFields().ToList();
        Constructors = standardClassType.GetConstructors().ToList();
    }

    static StandardClassInfo()
    {
        StandardClasses = GetStandardClasses();
    }

    private static Dictionary<string, ClassInfo> GetStandardClasses()
    {
        var asm = Assembly.GetExecutingAssembly();
        return new Dictionary<string, ClassInfo>(
            asm.GetTypes().Where(
                type => (type.IsClass || type.IsValueType) &&
                type.Namespace != null &&
                type.Namespace.StartsWith("OCompiler.StandardLibrary")
            ).Select(t => new KeyValuePair<string, ClassInfo>(t.Name, new StandardClassInfo(t)))
        );
    }

    public override string? GetMethodReturnType(string name, List<string> argumentTypes)
    {
        var candidates = Methods.Where(
            m => m.Name == name &&
            m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(argumentTypes)
        ).ToList();
        if (candidates.Count > 1)
        {
            throw new Exception($"More than one method matches signature {name}({string.Join(",", argumentTypes)})");
        }
        if (candidates.Count == 0)
        {
            return null;
        }

        var method = candidates[0];
        return method.ReturnType.Name;
    }

    public override bool HasField(string name)
    {
        var field = Fields.Where(f => f.Name == name).FirstOrDefault();
        return field != null;
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
