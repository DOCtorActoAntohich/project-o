using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class VariableDeclarationStatement : Statement
{
    public TypeReference? Type { get; set; }

    private DomExpression? _initExpression;

    public DomExpression? InitExpression
    {
        get => _initExpression;
        set
        {
            _initExpression = value;
            if (_initExpression != null)
            {
                _initExpression.Holder = this;
            }
        }
    }


    public VariableDeclarationStatement(string name)
    {
        Name = name;
    }
    public VariableDeclarationStatement(
        string name, 
        TypeReference? type = null, 
        DomExpression? initExpression = null) 
        : this(name)
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

        if (InitExpression != null)
        {
            stringBuilder.Append($" = {InitExpression}");
        }

        return stringBuilder.ToString();
    }
}