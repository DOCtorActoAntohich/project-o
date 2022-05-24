using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Tree;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    public Dictionary<string, ClassDeclaration> BuiltinClasses { get; }
    public Dictionary<string, ClassDeclaration> ParsedClasses { get; }

    public int ClassesCount => BuiltinClasses.Count + ParsedClasses.Count;

    public AnnotatedSyntaxTreeV2(Syntax.Tree syntaxTree)
    {
        var parsedClassTree = new ParsedClassTree(syntaxTree);
        BuiltinClasses = parsedClassTree.BuiltinClasses;
        ParsedClasses = parsedClassTree.ParsedClasses;

        var inheritanceTree = new InheritanceTree(this);

        ValidateTree();
    }

    public IEnumerable<string> AllNames()
    {
        foreach (var name in BuiltinClasses.Keys)
        {
            yield return name;
        }

        foreach (var name in ParsedClasses.Keys)
        {
            yield return name;
        }
    }
    
    public IEnumerable<ClassDeclaration> AllClasses()
    {
        foreach (var @class in BuiltinClasses.Values)
        {
            yield return @class;
        }

        foreach (var @class in ParsedClasses.Values)
        {
            yield return @class;
        }
    }

    public ClassDeclaration GetClass(string name)
    {
        if (BuiltinClasses.ContainsKey(name))
        {
            return BuiltinClasses[name];
        }

        if (ParsedClasses.ContainsKey(name))
        {
            return ParsedClasses[name];
        }

        throw new KeyNotFoundException($"The class {name} hasn't been found");
    }

    public bool HasClass(string name)
    {
        return BuiltinClasses.ContainsKey(name) || ParsedClasses.ContainsKey(name);
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        foreach (var declaration in ParsedClasses.Values)
        {
            stringBuilder.AppendLine(declaration.ToString());
        }
        
        return stringBuilder.ToString();
    }
}