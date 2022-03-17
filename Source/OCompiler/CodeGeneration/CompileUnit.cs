using System.CodeDom;
using System.Collections.Generic;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    private readonly CodeNamespace _codeNamespace;


    public CompileUnit(TreeValidator ast, string mainClass)
    {
        _codeNamespace = new CodeNamespace(ResultingNamespace);
        AddAllClasses(ast);
        AddEntryPoint(mainClass);
    }

    public CodeCompileUnit BuiltIn()
    {
        var compileUnit = new CodeCompileUnit();

        compileUnit.Namespaces.Add(_codeNamespace);
        
        return compileUnit;
    }
}