using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal abstract class Expression : CodeObject
{
    public DomStatement ParentStatement { get; set; } = null!;
    
    public Expression(string name) : base(name)
    {
    }
}