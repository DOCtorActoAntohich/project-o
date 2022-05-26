using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Compound;

internal class DictInitItem : Expression
{
    public Expression Key { get; set; }
    public Expression Value { get; set; }
    
    
    public DictInitItem(Expression key, Expression value) : base("")
    {
        Key = key;
        Value = value;
    }
}