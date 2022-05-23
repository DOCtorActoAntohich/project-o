namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class FieldReferenceExpression : Expression
{
    public Expression? SourceObject { get; set; }
    
    public FieldReferenceExpression(Expression? sourceObject, string name) : base(name)
    {
        SourceObject = sourceObject;
    }

    public override string ToString()
    {
        return $"{SourceObject}.{Name}";
    }
}