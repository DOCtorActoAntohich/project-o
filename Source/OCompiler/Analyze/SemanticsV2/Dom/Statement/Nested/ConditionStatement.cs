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
        Condition.Holder = this;
    }
    
    public ConditionStatement(DomExpression condition, IEnumerable<Statement> statements) : this(condition)
    {
        (this as ICanHaveStatements).AddStatements(statements);
    }
    
    public ConditionStatement(
        DomExpression condition, 
        IEnumerable<Statement> statements, 
        IEnumerable<Statement> elseStatements
        ) : this(condition, statements)
    {
        AddElseStatements(elseStatements);
    }

    public void AddElseStatement(Statement statement)
    {
        ElseStatements.Add(statement);
        statement.Holder = this;
    }
    
    public void AddElseStatements(IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            AddElseStatement(statement);
        }
    }
}