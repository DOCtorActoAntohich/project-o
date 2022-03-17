using System.Collections.Generic;
using System.Text;

using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Analyze.Lexical;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal class Constructor: IClassMember
{
    public List<Parameter> Parameters { get; }
    
    public Body Body { get; }

    public TokenPosition Position { get; }
    

    public static Constructor EmptyConstructor = new(new(), new(), new());
    
    public static bool TryParse(TokenEnumerator tokens, out Constructor? constructor)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.This)
        {
            constructor = null;
            return false;
        }
        var position = tokens.Current().Position;
        
        // Get next token.
        tokens.Next();
        // Parse parameters.
        var parameters = Method.Parameters.Parse(tokens);

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
        
        constructor = new Constructor(parameters, body, position);
        return true;
    }

    private Constructor(List<Parameter> parameters, Body body, TokenPosition position)
    {
        Parameters = parameters;
        Body = body;
        Position = position;
    }
    
    public string ToString(string prefix)
    {
        var @string = new StringBuilder();

        var definition = $"this({Method.Parameters.ToString(Parameters)})";

        @string.Append(definition);
        @string.AppendLine();
        @string.Append(Body.ToString(prefix));

        return @string.ToString();
    }
}
