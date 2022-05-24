namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;

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