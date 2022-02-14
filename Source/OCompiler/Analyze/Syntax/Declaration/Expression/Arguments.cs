using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal static class Arguments
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out List<Expression>? arguments)
    {
        if (tokens.Current() is not Lexical.Tokens.Delimiters.LeftParenthesis)
        {
            arguments = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        
        // Parse parameters.
        arguments = new List<Expression>();
        while (Expression.TryParse(tokens, out Expression? argument))
        {
            // Success.
            arguments.Add(argument!);
            
            // Continue if comma.
            if (tokens.Current() is Lexical.Tokens.Delimiters.Comma)
            {
                // Get next token.
                tokens.Next();
            }
            
            // Stop if no comma.
            break;
        }

        if (tokens.Current() is not Lexical.Tokens.Delimiters.RightParenthesis)
        {
            throw new Exception($"Expected ')' at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        
        return true;
    }
}
