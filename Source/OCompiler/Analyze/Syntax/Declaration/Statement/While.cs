using System;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class While : IStatement
{
    public Body Body { get; }
    public Expression.Expression Condition { get; }
    
    public static bool TryParse(TokenEnumerator tokens, out While? @while)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.While)
        {
            @while = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        // Try parse expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            throw new Exception($"Expression expected at line {tokens.Current().Position.Line}.");
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Loop)
        {
            throw new Exception($"Keyword 'loop' expected at line {tokens.Current().Position.Line}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        var body = new Body(tokens);
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Keyword 'end' expected at line {tokens.Current().Position.Line}.");
        }
        
        // Get next token.
        tokens.Next();
        
        @while = new While(expression!, body!);
        return true;
    }
    
    private While(Expression.Expression condition, Body body)
    {
        Condition = condition;
        Body = body;
    }

    public string ToString(string prefix)
    {
        var @string = new StringBuilder();

        @string.AppendLine($"while {Condition}");
        @string.Append(Body.ToString(prefix));
        
        return @string.ToString();
    }
}
