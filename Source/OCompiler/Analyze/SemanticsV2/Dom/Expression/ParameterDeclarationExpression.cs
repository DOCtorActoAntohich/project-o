using OCompiler.Analyze.SemanticsV2.Dom.Type;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class ParameterDeclarationExpression : Expression
{
    public TypeReference Type { get; set; }
    
    public ParameterDeclarationExpression(string name, TypeReference type) : base(name)
    {
        Type = type;
    }
}