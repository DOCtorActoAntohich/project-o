using System.Collections.Generic;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;

internal class LoopStatement : Statement, ICanHaveStatements
{
    public DomExpression Condition { get; set; }

    public List<Statement> Statements { get; } = new();

    public LoopStatement(DomExpression condition)
    {
        Condition = condition;
    }
    
    public LoopStatement(DomExpression condition, IEnumerable<Statement> statements) : this(condition)
    {
        Statements.AddRange(statements);
    }
}