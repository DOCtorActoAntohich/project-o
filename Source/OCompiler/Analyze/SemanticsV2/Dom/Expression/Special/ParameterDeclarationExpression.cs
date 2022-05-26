using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;

internal class ParameterDeclarationExpression : Expression
{
    public TypeReference Type { get; set; }

    private new DomStatement ParentStatement { get; set; } = null!; 
    
    
    // Always set when added to parameters collection.
    public ParametersCollection ParentCollection { get; set; } = null!;
    public CallableMember Owner => ParentCollection.Holder;

    public ParameterDeclarationExpression(string name, TypeReference type) : base(name)
    {
        Type = type;
    }

    public override string ToString()
    {
        return $"{Name}: {Type}";
    }
}