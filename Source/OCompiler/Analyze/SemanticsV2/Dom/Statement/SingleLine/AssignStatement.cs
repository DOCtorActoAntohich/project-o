using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class AssignStatement : Statement
{
    private DomExpression _lvalue = null!;
    private DomExpression _rvalue = null!;

    public DomExpression LValue
    {
        get => _lvalue;
        set
        {
            _lvalue = value;
            _lvalue.ParentStatement = this;
        }
    }

    public DomExpression RValue
    {
        get => _rvalue;
        set
        {
            _rvalue = value;
            _rvalue.ParentStatement = this;
        }
    }

    public AssignStatement(DomExpression lValue, DomExpression rValue)
    {
        LValue = lValue;
        RValue = rValue;
    }

    public string ToString(string prefix = "")
    {
        return $"{prefix}{LValue} = {RValue}";
    }
}