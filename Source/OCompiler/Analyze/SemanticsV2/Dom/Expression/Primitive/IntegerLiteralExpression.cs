namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;

internal class IntegerLiteralExpression : PrimitiveLiteralExpression
{
    public int Value { get; set; }

    public IntegerLiteralExpression(int value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}