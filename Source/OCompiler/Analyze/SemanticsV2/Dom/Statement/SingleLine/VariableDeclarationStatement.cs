using OCompiler.Analyze.SemanticsV2.Dom.Type;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;

internal class VariableDeclarationStatement : Statement
{
    public TypeReference Type { get; set; }

    public DomExpression InitExpression { get; set; }
    
    
    public VariableDeclarationStatement(string name, TypeReference type, DomExpression initExpression)
    {
        Name = name;
        Type = type;
        InitExpression = initExpression;
    }
}