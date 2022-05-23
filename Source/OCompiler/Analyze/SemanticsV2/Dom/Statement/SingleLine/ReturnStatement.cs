using System.Text;
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

    public string ToString(string prefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append("return");

        if (IsVoidMethodReturn)
        {
            return stringBuilder.ToString();
        }

        stringBuilder.Append($" {Expression}");

        return stringBuilder.ToString();
    }
}