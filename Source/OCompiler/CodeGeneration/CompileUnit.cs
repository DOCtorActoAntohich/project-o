using System.CodeDom;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration;

internal static partial class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    
    public static CodeCompileUnit FromAnnotatedSyntaxTree(TreeValidator ast)
    {
        var compileUnit = new CodeCompileUnit();
        
        var @namespace = new CodeNamespace(ResultingNamespace);
        @namespace.AddAllClasses(ast);
        compileUnit.Namespaces.Add(@namespace);
        
        return compileUnit;
    }
}