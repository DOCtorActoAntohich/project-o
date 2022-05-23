using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement;

internal interface ICanHaveStatements
{
    public List<Statement> Statements { get; }

    public void AddStatement(Statement statement)
    {
        Statements.Add(statement);
        statement.Holder = this;
    }

    public void AddStatements(IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            AddStatement(statement);
        }
    }
}