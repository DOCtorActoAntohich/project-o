using System.Collections.Generic;
using System.Text;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;

internal class LoopStatement : Statement, ICanHaveStatements
{
    public DomExpression Condition { get; set; }

    public List<Statement> Statements { get; } = new();

    public LoopStatement(DomExpression condition)
    {
        Condition = condition;
        condition.Holder = this;
    }
    
    public LoopStatement(DomExpression condition, IEnumerable<Statement> statements) : this(condition)
    {
        (this as ICanHaveStatements).AddStatements(statements);
    }

    public new string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append($"while ({Condition})\n")
            .Append(ICanHaveStatements.StatementsString(Statements, nestedPrefix));

        return stringBuilder.ToString();
    }
}