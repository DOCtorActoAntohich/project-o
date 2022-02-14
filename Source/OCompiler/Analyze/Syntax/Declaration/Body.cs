using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Analyze.Syntax.Declaration;

internal static class Body
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out List<INestable>? body)
    {
        body = new List<INestable>();

        while (true)
        {
            if (Variable.TryParse(tokens, out Variable? variable))
            {
                body.Add(variable!);
                continue;
            }
            
            if (Statement.Statement.TryParse(tokens, out Statement.Statement? statement))
            {
                body.Add(statement!);
                continue;
            }

            if (Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
            {
                body.Add(expression!);
                continue;
            }

            return true;
        }
    }
}
