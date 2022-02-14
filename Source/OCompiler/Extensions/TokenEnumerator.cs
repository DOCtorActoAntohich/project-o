using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Extensions;

internal static class TokenEnumerator
{
    public static Token Next(this IEnumerator<Token> tokens, Boolean skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (tokens.MoveNext() && skipWhitespaces && tokens.Current is Whitespace) { }
        return tokens.Current;
    }
    
    public static Token Current(this IEnumerator<Token> tokens, Boolean skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (skipWhitespaces && tokens.Current is Whitespace && tokens.MoveNext()) { }
        return tokens.Current;
    }
}
