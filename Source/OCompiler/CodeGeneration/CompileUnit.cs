using System.CodeDom;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    private readonly CodeNamespace _codeNamespace;


    public CompileUnit(TreeValidator ast)
    {
        _codeNamespace = new CodeNamespace(ResultingNamespace);
        AddAllClasses(ast);
    }

    public CodeCompileUnit BuiltIn()
    {
        var compileUnit = new CodeCompileUnit();

        compileUnit.Namespaces.Add(_codeNamespace);
        
        return compileUnit;
    }
}