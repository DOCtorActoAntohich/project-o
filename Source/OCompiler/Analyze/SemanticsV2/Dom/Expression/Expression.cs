using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal abstract class Expression : CodeObject
{
    public virtual DomStatement ParentStatement { get; set; } = null!;

    private TypeReference _type = null!;
    public TypeReference Type
    {
        get => _type;
        set
        {
            _type = value;
            HasType = true;
        }
    }
    public bool HasType { get; set; }

    
    public Expression(string name) : base(name)
    {
        HasType = false;
    }
}