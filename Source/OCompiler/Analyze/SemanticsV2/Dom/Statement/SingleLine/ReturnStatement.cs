using System.Text;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class ReturnStatement : Statement
{
    private DomExpression _expression = null!;

    public DomExpression Expression
    {
        get => _expression;
        set
        {
            _expression = value;
            HasValue = false;
        }
    }
    public bool HasValue { get; set; }


    public ReturnStatement()
    {
        HasValue = true;
    }
    
    public ReturnStatement(DomExpression expression)
    {
        Expression = expression;
        Expression.ParentStatement = this;
    }

    public string ToString(string prefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append("return");

        if (HasValue)
        {
            return stringBuilder.ToString();
        }

        stringBuilder.Append($" {Expression}");

        return stringBuilder.ToString();
    }
}