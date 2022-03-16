using System.CodeDom;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    private readonly CodeNamespace _codeNamespace;

    private CodeTypeDeclaration _currentTypeDeclaration;
    private CodeMemberMethod _currentCallable;
    
    public CompileUnit(TreeValidator ast)
    {
        _currentTypeDeclaration = new CodeTypeDeclaration();
        _currentCallable = new CodeMemberMethod();
        
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