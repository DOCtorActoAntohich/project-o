using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Constructor: IClassMember
{
    public List<Parameter> Parameters { get; }
    
    public Body Body { get; }

    public static Constructor EmptyConstructor = new(new(), new());
    
    public static bool TryParse(TokenEnumerator tokens, out Constructor? constructor)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.This)
        {
            constructor = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        // Parse parameters.
        var parameters = Method.Parameters.Parse(tokens);

        // Is.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Is)
        {
            throw new Exception($"Expected keyword 'is' at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        var body = new Body(tokens);
        
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

    private Constructor(List<Parameter> parameters, Body body)
    {
        Parameters = parameters;
        Body = body;
    }
    
    public string ToString(string prefix)
    {
        var @string = new StringBuilder();
        
        @string.AppendLine($"Constructor({Method.Parameters.ToString(Parameters)})");
        @string.Append(Body.ToString(prefix));

        return @string.ToString();
    }
}
