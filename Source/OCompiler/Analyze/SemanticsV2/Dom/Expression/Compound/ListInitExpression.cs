using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Compound;

internal class ListInitExpression : Expression
{
    public List<Expression> Items { get; } = new();
    
    public ListInitExpression() : base("")
    {
    }

    public ListInitExpression(IEnumerable<Expression> expressions) : this()
    {
        Items.AddRange(expressions);
    }
}