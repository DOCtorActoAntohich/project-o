using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class CallableMember : TypeMember
{
    public ParametersCollection Parameters { get; }
    public StatementsCollection Statements { get; }

    
    public CallableMember(string name) : base(name)
    {
        Parameters = new ParametersCollection(this);
        Statements = new StatementsCollection(this);
    }

    public bool SameSignatureAs(CallableMember other)
    {
        return Name == other.Name && Parameters.SameAs(other.Parameters);
    }

    public string CStyleForm()
    {
        return $"{Name}({Parameters.Count})";
    }
}