using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement;

internal interface ICanHaveStatements
{
    public List<Statement> Statements { get; }
}