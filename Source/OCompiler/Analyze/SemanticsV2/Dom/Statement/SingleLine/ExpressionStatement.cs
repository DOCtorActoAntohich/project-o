using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class ExpressionStatement : Statement
{
    public DomExpression Expression { get; set; }

    public ExpressionStatement(DomExpression expression)
    {
        Expression = expression;

        Expression.Holder = this;
    }

    public string ToString(string prefix = "")
    {
        return $"{prefix}{Expression}";
    }
}