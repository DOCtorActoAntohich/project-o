using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;


internal class ConditionStatement : Statement
{
    private DomExpression _condition = null!;

    public DomExpression Condition
    {
        get => _condition;
        set
        {
            _condition = value;
            _condition.ParentStatement = this;
        }
    }

    public StatementsCollection Statements { get; }
    public StatementsCollection ElseStatements { get; }

    public bool HasElseBlock => ElseStatements.Count > 0;


    public Dictionary<string, TypeReference> Context { get; } = new();

    private ConditionStatement()
    {
        Statements = new StatementsCollection(this);
        ElseStatements = new StatementsCollection(this);
    }
    
    public ConditionStatement(DomExpression condition) : this()
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

    public new string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append($"if ({Condition})");

        if (Statements.Count > 0)
        {
            stringBuilder.Append('\n')
                .Append(Statements.ToString(nestedPrefix));
        }

        if (!HasElseBlock)
        {
            return stringBuilder.ToString();
        }

        stringBuilder
            .Append('\n').Append(prefix)
            .Append("else\n")
            .Append(ElseStatements.ToString(nestedPrefix));

        return stringBuilder.ToString();
    }
}