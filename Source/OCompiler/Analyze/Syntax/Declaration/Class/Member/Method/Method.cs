using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal class Method: IClassMember
{
    public Identifier Name { get; }
    public List<Parameter> Parameters { get; }
    public Identifier? ReturnType { get; }
    public Body Body { get; }
    
    public static bool TryParse(TokenEnumerator tokens, out Method? method)
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
            throw new SyntaxError(tokens.Current().Position, $"Expected method name");
        }
        
        // Get next token.
        tokens.Next();
        // Parse parameters.
        var parameters = Member.Method.Parameters.Parse(tokens);

        // Try to parse return type.
        Identifier? returnType = null;
        // Colon.
        if (tokens.Current() is Lexical.Tokens.Delimiters.Colon)
        {
            // Return type.
            if (tokens.Next() is not Identifier)
            {
                throw new SyntaxError(tokens.Current().Position, "Expected class name");
            }
            
            returnType = (Identifier)tokens.Current();
            // Get next token.
            tokens.Next();
        }
        
        // Is.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Is)
        {
            throw new SyntaxError(tokens.Current().Position, "Expected 'is' keyword");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        var body = new Body(tokens);
        
        // End.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new SyntaxError(tokens.Current().Position, "Expected 'end' keyword");
        }
        
        // Get next token.
        tokens.Next();
        
        method = new Method(name, parameters, returnType, body!);
        return true;
    }

    private Method(Identifier name, List<Parameter> parameters, Identifier? returnType, Body body)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }
    
    public string ToString(string prefix)
    {
        var @string = new StringBuilder();

        string returnType = ReturnType is null ? "None" : ReturnType.Literal;
        @string.AppendLine($"{Name.Literal}({Member.Method.Parameters.ToString(Parameters)}) -> {returnType}");
        @string.Append(Body.ToString(prefix));
        
        return @string.ToString();
    }
}
