using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Constructor: IMember
{
    public List<Parameter>? Parameters { get; }
    
    public List<INestable> Body { get; }
    
    public static Boolean TryParse(TokenEnumerator tokens, out Constructor? constructor)
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
    
    public String ToString(String prefix)
    {
        StringBuilder @string = new StringBuilder();
        
        @string.AppendLine($"Constructor({Member.Method.Parameters.ToString(Parameters)})");
        @string.Append(Declaration.Body.ToString(Body, prefix));

        return @string.ToString();
    }
}
