using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Field: Variable, IMember
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out Field? field)
    {
        Boolean status = Variable.TryParse(tokens, out Variable? variable);
        field = (Field?) variable;
        return status;
    }
    
    protected Field(Identifier name, Expression.Expression expression): base(name, expression) { } 
}
