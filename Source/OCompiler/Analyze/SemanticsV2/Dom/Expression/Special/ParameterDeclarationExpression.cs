using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;

internal class ParameterDeclarationExpression : Expression
{
    public TypeReference Type { get; set; }
    
    public new TypeMember? Holder { get; set; }
    
    public ParameterDeclarationExpression(string name, TypeReference type) : base(name)
    {
        Type = type;
    }

    public override string ToString()
    {
        return $"{Name}: {Type}";
    }
}