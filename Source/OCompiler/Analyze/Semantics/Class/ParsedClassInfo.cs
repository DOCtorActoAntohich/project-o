using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics.Class;

internal class ParsedClassInfo : ClassInfo
{
    public override Syntax.Declaration.Class.Class? Class { get; }
    public override ClassInfo? BaseClass { get; }
    public List<ParsedMethodInfo> Methods { get; } = new();
    public List<ParsedFieldInfo> Fields { get; } = new();
    public List<ParsedConstructorInfo> Constructors { get; } = new();

    private readonly static Dictionary<string, ParsedClassInfo> parsedClasses = new();

    private ParsedClassInfo(Syntax.Declaration.Class.Class parsedClass)
    {
        AddMethods(parsedClass.Methods);
        AddFields(parsedClass.Fields);
        AddConstructors(parsedClass.Constructors);

        Name = parsedClass.Name.Literal;
        Class = parsedClass;
        if (parsedClass.Extends != null)
        {
            BaseClass = GetByName(parsedClass.Extends.Literal);
        }
    }

    protected ParsedClassInfo()
    {
    }

    private void AddMethods(List<Method> methods)
    {
        foreach (var method in methods)
        {
            var methodInfo = new ParsedMethodInfo(method);
            var methodName = methodInfo.Name;
            var parameterTypes = methodInfo.GetParameterTypes();
            if (HasMethod(methodName, parameterTypes))
            {
                var argsStr = string.Join(", ", parameterTypes);
                var @return = methodInfo.ReturnType == "Void" ? "" : $"-> {methodInfo.ReturnType}"; 
                throw new Exception($"Method {methodName}({argsStr}) {@return} defined more than once in class {Name}");
            }
            Methods.Add(methodInfo);
        }
    }

    private void AddFields(List<Field> fields)
    {
        foreach (var field in fields)
        {
            var fieldInfo = new ParsedFieldInfo(field);
            if (Fields.Where(f => f.Name == fieldInfo.Name).Any())
            {
                throw new Exception($"Field {fieldInfo.Name} defined more than once in class {Name}");
            }
            Fields.Add(fieldInfo);
        }
    }

    private void AddConstructors(List<Constructor> constructors)
    {
        foreach (var constructor in constructors)
        {
            var constructorInfo = new ParsedConstructorInfo(constructor);
            var parameterTypes = constructorInfo.GetParameterTypes();
            if (HasConstructor(parameterTypes))
            {
                var argsStr = string.Join(", ", parameterTypes);
                throw new Exception($"Constructor {Name}({argsStr}) defined more than once in class {Name}");
            }
            Constructors.Add(constructorInfo);
        }
    }

    public static ParsedClassInfo GetByClass(Syntax.Declaration.Class.Class parsedClass)
    {
        var name = parsedClass.Name.Literal;
        if (parsedClasses.TryGetValue(name, out var classInfo) && classInfo is not EmptyParsedClassInfo)
        {
            return classInfo;
        }

        var newInfo = new ParsedClassInfo(parsedClass);
        parsedClasses[name] = newInfo;
        return newInfo;
    }

    public static ClassInfo GetByName(string name)
    {
        if (parsedClasses.ContainsKey(name))
        {
            return parsedClasses[name];
        }
        if (BuiltClassInfo.StandardClasses.ContainsKey(name))
        {
            return BuiltClassInfo.StandardClasses[name];
        }

        var newClassInfo = new EmptyParsedClassInfo(name);
        parsedClasses.Add(name, newClassInfo);
        return newClassInfo;
    }

    public override string? GetMethodReturnType(string name, List<string> argumentTypes)
    {
        var method = Methods.Where(
            m => m.Name == name &&
            m.Parameters.Select(p => p.Type).SequenceEqual(argumentTypes)
        ).FirstOrDefault();

        return method?.ReturnType;
    }

    public bool HasMethod(string name, List<string> argumentTypes)
    {
        return GetMethodReturnType(name, argumentTypes) != null;
    }


    public ParsedFieldInfo? GetFieldInfo(string name)
    {
        var field = Fields.Where(f => f.Name == name).FirstOrDefault();
        return field;
    }

    public override string? GetFieldType(string name)
    {
        return GetFieldInfo(name)?.Type;
    }

    public override bool HasField(string name)
    {
        return GetFieldInfo(name) != null;
    }

    public void AddFieldType(string name, string type)
    {
        var field = Fields.Where(f => f.Name == name).FirstOrDefault();
        if (field != null && field.Type == null)
        {
            field.Type = type;
        }
    }

    public override bool HasConstructor(List<string> argumentTypes)
    {
        var constructor = Constructors.Where(
            c => c.Parameters.Select(p => p.Type).SequenceEqual(argumentTypes)
        ).FirstOrDefault();

        return constructor != null;
    }

    public override string ToString()
    {
        StringBuilder @string = new();
        @string.Append("Parsed class ");
        @string.Append(Name);
        if (BaseClass != null)
        {
            @string.Append(" extends ");
            @string.Append(BaseClass);
        }
        return @string.ToString();
    }
}
