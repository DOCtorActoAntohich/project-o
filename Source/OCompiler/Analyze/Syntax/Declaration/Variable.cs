using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Variable: Assignment
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out Variable? variable)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Var)
        {
            variable = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        
        // Parse statement.
        if (!Assignment.TryParse(tokens, out Assignment? assignment))
        {
            throw new Exception($"Expected assignment name at position {tokens.Current().StartOffset}.");
        }

        variable = new Variable(assignment!.Identifier, assignment.Expression);
        return true;
    }

    protected Variable(Identifier name, Expression.Expression expression): base(name, expression){ } 
}
