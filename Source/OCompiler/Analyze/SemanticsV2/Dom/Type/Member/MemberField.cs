namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

internal class MemberField : TypeMember
{
    public TypeReference Type { get; set; }
    
    public DomExpression InitExpression { get; set; }
    
    public MemberField(string name, TypeReference type, DomExpression initExpression) : base(name)
    {
        Type = type;
        InitExpression = initExpression;
    }
}