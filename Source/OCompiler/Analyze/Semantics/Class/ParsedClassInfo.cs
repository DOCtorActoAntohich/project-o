using System;
using System.Collections.Generic;
using System.Linq;

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
    public List<Method> Methods => Class!.Methods;
    public List<Field> Fields => Class!.Fields;
    public List<Constructor> Constructors => Class!.Constructors;

    private Dictionary<string, ParsedClassInfo> parsedClasses = new();

    public ParsedClassInfo(Syntax.Declaration.Class.Class parsedClass)
    {
        if (parsedClasses.TryGetValue(Name, out var classInfo) && classInfo is not EmptyParsedClassInfo)
        {
            return;
        }

        Name = parsedClass.Name.Literal;
        Class = parsedClass;
        if (parsedClass.Extends != null)
        {
            BaseClass = GetByName(parsedClass.Extends.Literal);
        }
        parsedClasses[Name] = this;
    }

    public ParsedClassInfo()
    {
    }

    public ClassInfo GetByName(string name)
    {
        if (parsedClasses.ContainsKey(name))
        {
            return parsedClasses[name];
        }
        if (StandardClassInfo.StandardClasses.ContainsKey(name))
        {
            return StandardClassInfo.StandardClasses[name];
        }

        var newClassInfo = new EmptyParsedClassInfo(name);
        parsedClasses.Add(name, newClassInfo);
        return newClassInfo;
    }

    public override string? GetMethodReturnType(string name, List<string> argumentTypes)
    {
        var candidates = Methods.Where(
            m => m.Name.Literal == name &&
            m.Parameters.Select(p => p.Type.Literal).SequenceEqual(argumentTypes)
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
        return method.ReturnType == null ? "Void" : method.ReturnType.Literal;
    }

    public override bool HasField(string name)
    {
        var field = Fields.Where(f => f.Identifier.Literal == name).FirstOrDefault();
        return field != null;
    }

    public override bool HasConstructor(List<string> argumentTypes)
    {
        var candidates = Constructors.Where(
            c => c.Parameters.Select(p => p.Type.Literal).SequenceEqual(argumentTypes)
        ).ToList();
        if (candidates.Count > 1)
        {
            throw new Exception($"More than one constructor matches signature {Name}({string.Join(",", argumentTypes)})");
        }

        return candidates.Count == 1;
    }
}
