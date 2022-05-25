using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class VariableDeclarationStatement : Statement
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
    
    
    public VariableDeclarationStatement(string name)
    {
        Name = name;
        HasTypeAnnotation = false;
        HasInitExpression = false;
    }

    public VariableDeclarationStatement(string name, TypeReference typeAnnotation) : this(name)
    {
        Type = typeAnnotation;
    }

    public VariableDeclarationStatement(string name, DomExpression initExpression) : this(name)
    {
        InitExpression = initExpression;
    }

    public VariableDeclarationStatement(string name, TypeReference typeAnnotation, DomExpression initExpression)
        : this(name)
    {
        Type = typeAnnotation;
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
}