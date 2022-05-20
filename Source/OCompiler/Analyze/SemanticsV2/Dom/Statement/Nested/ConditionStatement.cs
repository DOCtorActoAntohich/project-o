using System.Collections.Generic;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;


internal class ConditionStatement : Statement, ICanHaveStatements
{
    public DomExpression Condition { get; set; }

    public List<Statement> Statements { get; } = new();

    public List<Statement> ElseStatements { get; } = new();

    public ConditionStatement(DomExpression condition)
    {
        Condition = condition;
    }
    
    public ConditionStatement(DomExpression condition, IEnumerable<Statement> statements) : this(condition)
    {
        Statements.AddRange(statements);
    }
    
    public ConditionStatement(
        DomExpression condition, 
        IEnumerable<Statement> statements, 
        IEnumerable<Statement> elseStatements
        ) : this(condition, statements)
    {
        ElseStatements.AddRange(elseStatements);
    }
}