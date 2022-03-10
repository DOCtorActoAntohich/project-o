using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Field: Variable, IClassMember
{
    public static bool TryParse(TokenEnumerator tokens, out Field? field)
    {
        bool result = Variable.TryParse(tokens, out var variable);
        field = (Field)variable!;
        return result;
    }
    
    protected Field(Identifier name, Expression.Expression expression): base(name, expression) { }
}
