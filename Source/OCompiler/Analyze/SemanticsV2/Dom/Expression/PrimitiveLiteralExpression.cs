namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class PrimitiveLiteralExpression : Expression
{
    public string Literal => Name;
    
    public PrimitiveLiteralExpression(string expression) : base(expression)
    {
    }
}