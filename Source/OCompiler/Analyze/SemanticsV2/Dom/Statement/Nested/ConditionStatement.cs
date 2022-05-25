using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;


internal class ConditionStatement : Statement
{
    public DomExpression Condition { get; set; } = null!;

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
        Condition.ParentStatement = this;
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
            .Append($"if ({Condition})\n")
            .Append(Statements.ToString(nestedPrefix));


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