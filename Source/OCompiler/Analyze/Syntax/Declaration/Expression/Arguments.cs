using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal static class Arguments
{
    public static bool TryParse(TokenEnumerator tokens, out List<Expression>? arguments)
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
                continue;
            }
            
            // Stop if no comma.
            break;
        }

        if (tokens.Current() is not Lexical.Tokens.Delimiters.RightParenthesis)
        {
            throw new Exception($"Expected ')' at line {tokens.Current().Position.Line}.");
        }
        
        // Get next token.
        tokens.Next();
        
        return true;
    }

    public static string ToString(List<Expression>? arguments)
    {
        if (arguments is null)
        {
            return "";
        }
        
        var @string = new StringBuilder();
        for (var i = 0; i < arguments.Count; ++i)
        {
            @string.Append(arguments[i].ToString());
            if (i + 1 != arguments.Count)
            {
                @string.Append(", ");
            }
        }

        return $"({@string})";
    }
}
