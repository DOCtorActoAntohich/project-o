using System.CodeDom;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit2
{
    public const string ResultingNamespace = "OLang";

    private readonly CodeNamespace _codeNamespace;

    private CodeTypeDeclaration _typeDeclaration;
    
    public CompileUnit2(TreeValidator ast)
    {
        _codeNamespace = new CodeNamespace(ResultingNamespace);
        _typeDeclaration = new CodeTypeDeclaration();

        AddAllClasses(ast);
    }

    public CodeCompileUnit BuiltIn()
    {
        var compileUnit = new CodeCompileUnit();

        compileUnit.Namespaces.Add(_codeNamespace);
        
        return compileUnit;
    }
}