namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class ParameterReferenceExpression : Expression
{
    public ParameterReferenceExpression(string name) : base(name)
    {
    }

    public override string ToString()
    {
        return Name;
    }
}