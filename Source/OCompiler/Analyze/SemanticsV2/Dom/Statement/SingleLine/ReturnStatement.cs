using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class ReturnStatement : Statement
{
    private DomExpression? _expression;

    public DomExpression? Expression
    {
        get => _expression;
        set
        {
            _expression = value;
            if (_expression != null)
            {
                _expression.Holder = this;
            }
        }
    }
    
    public bool IsVoidMethodReturn => _expression == null;

    public ReturnStatement(DomExpression? expression = null)
    {
        Expression = expression;
    }
}