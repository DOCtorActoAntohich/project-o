using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Exceptions.Semantic;

internal class DuplicateParameterError : AnalyzeError
{
    public DuplicateParameterError(ClassDeclaration @class, CallableMember callable, string parameterName)
        : base($"Duplicate parameter in {@class.Name}::{callable.CStyleForm()}): {parameterName}")
    {
        
    }
    
    public DuplicateParameterError(string message) : base(message)
    {
    }
}