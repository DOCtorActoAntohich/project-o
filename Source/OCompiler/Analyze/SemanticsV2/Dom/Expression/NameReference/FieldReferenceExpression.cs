using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;

internal class FieldReferenceExpression : Expression
{
    public Expression SourceObject { get; set; }

    public MemberField Field { get; set; } = null!;

    public FieldReferenceExpression(Expression sourceObject, string name) : base(name)
    {
        SourceObject = sourceObject;
    }

    public override string ToString()
    {
        return $"{SourceObject}.{Name}";
    }
}