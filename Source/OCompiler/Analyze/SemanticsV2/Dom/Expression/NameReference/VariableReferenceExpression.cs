namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;

internal class VariableReferenceExpression : Expression
{
    public VariableReferenceExpression(string name) : base(name)
    {
    }

    public override string ToString()
    {
        return Name;
    }
}