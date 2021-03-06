using System.Globalization;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;

internal class RealLiteralExpression : PrimitiveLiteralExpression
{
    public double Value { get; set; }

    public RealLiteralExpression(double value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}