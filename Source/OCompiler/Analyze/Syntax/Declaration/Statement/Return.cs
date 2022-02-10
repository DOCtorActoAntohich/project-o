using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Return: Statement
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out Return? @return)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Return)
        {
            @return = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        Declaration.Expression.Expression.TryParse(tokens, out Declaration.Expression.Expression? expression);

        @return = new Return(expression);
        return true;
    }

    private Return(Expression.Expression? expression): base(expression) { }
}
