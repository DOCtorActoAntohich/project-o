using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class AssignStatement : Statement
{
    public DomExpression LValue { get; set; }
    public DomExpression RValue { get; set; }
    
    public AssignStatement(DomExpression lValue, DomExpression rValue)
    {
        LValue = lValue;
        RValue = rValue;

        LValue.Holder = this;
        RValue.Holder = this;
    }

    public string ToString(string prefix = "")
    {
        return $"{prefix}{LValue} = {RValue}";
    }
}