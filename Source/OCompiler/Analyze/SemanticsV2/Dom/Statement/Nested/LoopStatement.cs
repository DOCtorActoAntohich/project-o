using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;

internal class LoopStatement : Statement
{
    public DomExpression Condition { get; set; } = null!;

    public StatementsCollection Statements { get; }

    public Dictionary<string, TypeReference> Context { get; } = new();

    private LoopStatement()
    {
        Statements = new StatementsCollection(this);
    }
    
    public LoopStatement(DomExpression condition) : this()
    {
        Condition = condition;
        condition.ParentStatement = this;
    }
    
    public LoopStatement(DomExpression condition, IEnumerable<Statement> statements) : this(condition)
    {
        Statements.AddRange(statements);
    }

    public new string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append($"while ({Condition})\n")
            .Append(Statements.ToString(nestedPrefix));

        return stringBuilder.ToString();
    }
}