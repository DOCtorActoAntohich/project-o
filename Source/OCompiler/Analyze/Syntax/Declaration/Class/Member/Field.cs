using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Field: Variable, IClassMember
{
    public static bool TryParse(TokenEnumerator tokens, out Field? field)
    {
        if (!Variable.TryParse(tokens, out var variable))
        {
            field = null;
            return false;
        }

        field = new Field(variable!);
        return true;
    }

    protected Field(Identifier name, TypeAnnotation type, Expression.Expression expression) : base(name, type, expression) { }

    protected Field(Variable variable) : base(variable.Identifier, variable.Type, variable.Expression) { }
}
