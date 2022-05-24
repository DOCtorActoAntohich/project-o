using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Compound;

internal class DictInitExpression : Expression
{
    public List<DictInitItem> Items { get; } = new();
    
    public DictInitExpression() : base("")
    {
    }

    public DictInitExpression(IEnumerable<DictInitItem> expressions) : this()
    {
        Items.AddRange(expressions);
    }
}