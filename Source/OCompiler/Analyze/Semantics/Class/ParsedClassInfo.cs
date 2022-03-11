using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

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
        Methods = parsedClass.Methods.Select(method => new ParsedMethodInfo(method)).ToList();
        Fields = parsedClass.Fields.Select(field => new ParsedFieldInfo(field)).ToList();
        Constructors = parsedClass.Constructors.Select(constructor => new ParsedConstructorInfo(constructor)).ToList();

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
        var candidates = Methods.Where(
            m => m.Name == name &&
            m.Parameters.Select(p => p.Type).SequenceEqual(argumentTypes)
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
        return method.ReturnType;
    }

    public override bool HasField(string name)
    {
        var field = Fields.Where(f => f.Name == name).FirstOrDefault();
        return field != null;
    }

    public override bool HasConstructor(List<string> argumentTypes)
    {
        var candidates = Constructors.Where(
            c => c.Parameters.Select(p => p.Type).SequenceEqual(argumentTypes)
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
