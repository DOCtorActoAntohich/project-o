using ThisToken = OCompiler.Analyze.Lexical.Tokens.Keywords.This;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;

internal class ThisReferenceExpression : Expression
{
    public ThisReferenceExpression() : base(ThisToken.Literal)
    {
    }

    public override string ToString()
    {
        return Name;
    }
}