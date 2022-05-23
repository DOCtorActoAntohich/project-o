using System.Text;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

internal class MemberField : TypeMember
{
    public TypeReference? Type { get; set; }
    
    public DomExpression? InitExpression { get; set; }
    public bool IsInitialized => InitExpression != null;

    public MemberField(string name, TypeReference? type = null, DomExpression? initExpression = null) : base(name)
    {
        Type = type;
        InitExpression = initExpression;
    }

    public string ToString(string prefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append(Name);

        if (Type != null)
        {
            stringBuilder.Append($": {Type}");
        }

        if (IsInitialized)
        {
            stringBuilder.Append($" = {InitExpression}");
        }

        return stringBuilder.ToString();
    }
}