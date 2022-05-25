using System.Text;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

internal class MemberField : TypeMember
{
    private TypeReference _type = null!;
    private DomExpression _initExpression = null!;
    public TypeReference Type
    {
        get => _type;
        set
        {
            _type = value;
            HasTypeAnnotation = true;
        } 
    }

    public DomExpression InitExpression
    {
        get => _initExpression;
        set
        {
            _initExpression = value;
            HasInitExpression = true;
        }
    }

    public bool HasTypeAnnotation { get; set; }
    public bool HasInitExpression { get; set; }


    public MemberField(string name) : base(name)
    {
        HasTypeAnnotation = false;
        HasInitExpression = false;
    }

    public MemberField(string name, TypeReference type) : this(name)
    {
        Type = type;
    }
    
    public MemberField(string name, DomExpression initExpression) : this(name)
    {
        InitExpression = initExpression;
    }

    public MemberField(string name, TypeReference type, DomExpression initExpression) : this(name)
    {
        Type = type;
        InitExpression = initExpression;
    }

    public string ToString(string prefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append(Name);

        if (HasTypeAnnotation)
        {
            stringBuilder.Append($": {Type}");
        }

        if (HasInitExpression)
        {
            stringBuilder.Append($" = {InitExpression}");
        }

        return stringBuilder.ToString();
    }

    public bool SameNameAs(MemberField other)
    {
        return Name == other.Name;
    }

    public bool DifferentNameFrom(MemberField other)
    {
        return !SameNameAs(other);
    }

    public override int GetHashCode()
    {
        return Owner.GetHashCode() ^ Name.GetHashCode();
    }
}