using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Field: Variable, IMember
{
    public static Boolean TryParse(TokenEnumerator tokens, out Field? field)
    {
        if (Variable.TryParse(tokens, out Variable? variable))
        {
            field = new Field(variable!.Identifier, variable.Expression);
            return true;
        }

        field = null;
        return false;
    }
    
    private Field(Identifier name, Expression.Expression expression): base(name, expression) { }
}
