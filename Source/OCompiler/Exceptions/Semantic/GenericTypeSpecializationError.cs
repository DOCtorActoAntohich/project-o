using OCompiler.Analyze.SemanticsV2.Dom.Type;

namespace OCompiler.Exceptions.Semantic;

internal class GenericTypeSpecializationError : AnalyzeError
{
    public GenericTypeSpecializationError(ClassDeclaration @class, TypeReference type) 
        : base($"In class {@class.Name}: Type `{type.Name}` must have no specialization")
    {
    }
}