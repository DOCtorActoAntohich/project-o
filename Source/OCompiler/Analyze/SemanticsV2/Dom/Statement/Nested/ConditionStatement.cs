using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;


internal class ConditionStatement : Statement, ICanHaveStatements
{
    public DomExpression Condition { get; set; }

    public List<Statement> Statements { get; } = new();
    public List<Statement> ElseStatements { get; } = new();
    
    public bool HasElseBlock => ElseStatements.Count > 0;


    public Dictionary<string, TypeReference> Context { get; } = new();

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

    public new string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append($"if ({Condition})\n")
            .Append(ICanHaveStatements.StatementsString(Statements, nestedPrefix));
        

        if (!HasElseBlock)
        {
            return stringBuilder.ToString();
        }

        stringBuilder
            .Append('\n').Append(prefix)
            .Append("else\n")
            .Append(ICanHaveStatements.StatementsString(ElseStatements, nestedPrefix));

        return stringBuilder.ToString();
    }
}