using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class RValueStatement : Statement
{
    public DomExpression Expression;

    public RValueStatement(DomExpression expression)
    {
        Expression = expression;
    }
}