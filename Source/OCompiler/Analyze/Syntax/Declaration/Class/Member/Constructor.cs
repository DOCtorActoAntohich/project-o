using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Constructor: IMember
{
    public List<Parameter>? Parameters { get; }
    
    public List<INestable> Body { get; }
    
    public static Boolean TryParse(IEnumerator<Token> tokens, out Constructor? constructor)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.This)
        {
            constructor = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        // Try Parse parameters.
        Member.Method.Parameters.TryParse(tokens, out List<Parameter>? parameters);

        // Is.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Is)
        {
            throw new Exception($"Expected keyword 'is' at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        if (!Declaration.Body.TryParse(tokens, out List<INestable>? body))
        {
            throw new Exception($"Expected body at position {tokens.Current().StartOffset}.");
        }
        
        // End.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Expected keyword 'end' at position {tokens.Current().StartOffset}.");
        }
    
        // Get next token.
        tokens.Next();
        
        constructor = new Constructor(parameters, body!);
        return true;
    }

    private Constructor(List<Parameter>? parameters, List<INestable> body)
    {
        Parameters = parameters;
        Body = body;
    }
}
