namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;

internal class BooleanLiteralExpression : PrimitiveLiteralExpression
{
    public bool Value { get; set; }

    public BooleanLiteralExpression(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}