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
            _expression.ParentStatement = this;
            HasValue = true;
        }
    }
    public bool HasValue { get; set; }


    public ReturnStatement()
    {
        HasValue = false;
    }
    
    public ReturnStatement(DomExpression expression) : this()
    {
        Expression = expression;
    }

    public string ToString(string prefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append("return");

        if (!HasValue)
        {
            return stringBuilder.ToString();
        }

        stringBuilder.Append($" {Expression}");

        return stringBuilder.ToString();
    }
}