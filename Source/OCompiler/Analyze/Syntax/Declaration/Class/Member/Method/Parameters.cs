using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal static class Parameters
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out List<Parameter>? parameters)
    {
        if (tokens.Current() is not Lexical.Tokens.Delimiters.LeftParenthesis)
        {
            parameters = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        
        // Parse parameters.
        parameters = new List<Parameter>();
        while (Parameter.TryParse(tokens, out Parameter? parameter))
        {
            // Success.
            parameters.Add(parameter!);
            
            // Stop if no comma.
            if (tokens.Current() is not Lexical.Tokens.Delimiters.Comma)
            {
                break;
            }
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
