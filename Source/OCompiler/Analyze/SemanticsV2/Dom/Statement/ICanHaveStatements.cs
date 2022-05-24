using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement;

internal interface ICanHaveStatements
{
    public List<Statement> Statements { get; }
    
    public Dictionary<string, TypeReference> Context { get; }

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

    public static string StatementsString(
        List<Statement> statements,
        string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < statements.Count; ++i)
        {
            var isLast = (i + 1 == statements.Count);
            var firstLinePrefix = isLast ? nestedPrefix + "└── " : nestedPrefix + "├── ";
            stringBuilder.Append(
                statements[i].ToString(firstLinePrefix, nestedPrefix + "│   "));
            if (!isLast)
            {
                stringBuilder.Append('\n');
            }
        }
        
        return stringBuilder.ToString();
    }
}