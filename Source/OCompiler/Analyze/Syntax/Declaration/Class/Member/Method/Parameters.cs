using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal static class Parameters
{
    public static List<Parameter> Parse(TokenEnumerator tokens)
    {
        var parameters = new List<Parameter>();
        if (tokens.Current() is not Lexical.Tokens.Delimiters.LeftParenthesis)
        {
            return parameters;
        }
        
        // Get next token.
        tokens.Next();
        
        // Parse parameters.
        while (Parameter.TryParse(tokens, out Parameter? parameter))
        {
            // Success.
            parameters.Add(parameter!);
            
            // Stop if no comma.
            if (tokens.Current() is not Lexical.Tokens.Delimiters.Comma)
            {
                break;
            }
            tokens.Next();
        }

        if (tokens.Current() is not Lexical.Tokens.Delimiters.RightParenthesis)
        {
            throw new Exception($"Expected ')' at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        
        return parameters;
    }

    public static string ToString(List<Parameter>? parameters)
    {
        if (parameters is null)
        {
            return "";
        }
        
        var @string = new StringBuilder();
        for (var i = 0; i < parameters.Count; ++i)
        {
            @string.Append(parameters[i].ToString());
            if (i + 1 != parameters.Count)
            {
                @string.Append(", ");
            }
        }

        return @string.ToString();
    }
}
