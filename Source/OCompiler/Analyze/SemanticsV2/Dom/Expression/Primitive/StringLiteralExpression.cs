namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;

internal class StringLiteralExpression : PrimitiveLiteralExpression
{
    public string Value { get; set; }

    public StringLiteralExpression(string value)
    {
        Value = value;
    }
}