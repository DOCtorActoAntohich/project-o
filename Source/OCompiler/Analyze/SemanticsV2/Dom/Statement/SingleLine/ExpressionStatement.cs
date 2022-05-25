using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class ExpressionStatement : Statement
{
    private DomExpression _expression = null!;

    public DomExpression Expression
    {
        get => _expression;
        set
        {
            _expression = value;
            _expression.ParentStatement = this;
        }
    }

    public ExpressionStatement(DomExpression expression)
    {
        Expression = expression;
    }

    public string ToString(string prefix = "")
    {
        return $"{prefix}{Expression}";
    }
}