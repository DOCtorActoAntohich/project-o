using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;

namespace OCompiler.Analyze.SemanticsV2.Tree;

internal partial class ParsedClassTree
{
    public Dictionary<string, ClassDeclaration> BuiltinClasses { get; }
    public Dictionary<string, ClassDeclaration> ParsedClasses { get; } = new();

    public ParsedClassTree(Syntax.Tree syntaxTree)
    {
        BuiltinClasses = new BuiltinClassTree().Classes;
        
        CreateDeclarationsFrom(syntaxTree);
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