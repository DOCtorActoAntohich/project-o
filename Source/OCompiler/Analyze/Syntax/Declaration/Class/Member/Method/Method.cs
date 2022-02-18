using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal class Method: IMember
{
    public Identifier Name { get; }
    public List<Parameter>? Parameters { get; }
    public Identifier? ReturnType { get; }
    public List<INestable> Body { get; }
    
    public static Boolean TryParse(TokenEnumerator tokens, out Method? method)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Method)
        {
            method = null;
            return false;
        }
        
        // Name.
        if (tokens.Next() is not Identifier name)
        {
            throw new Exception($"Expected method name at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try Parse parameters.
        Member.Method.Parameters.TryParse(tokens, out List<Parameter>? parameters);

        // Try to parse return type.
        Identifier? returnType = null;
        // Colon.
        if (tokens.Current() is Lexical.Tokens.Delimiters.Colon)
        {
            // Return type.
            if (tokens.Next() is not Identifier)
            {
                throw new Exception($"Expected class name at position {tokens.Current().StartOffset}.");
            }
            
            returnType = (Identifier)tokens.Current();
            // Get next token.
            tokens.Next();
        }
        
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
        
        method = new Method(name, parameters, returnType, body!);
        return true;
    }

    private Method(Identifier name, List<Parameter>? parameters, Identifier? returnType, List<INestable> body)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }
    
    public String ToString(String prefix)
    {
        StringBuilder @string = new StringBuilder();

        String returnType = ReturnType is null ? "None" : ReturnType.Literal;
        @string.AppendLine($"{Name.Literal}({Member.Method.Parameters.ToString(Parameters)}) -> {returnType}");
        @string.Append(Declaration.Body.ToString(Body, prefix));
        
        return @string.ToString();
    }
}
