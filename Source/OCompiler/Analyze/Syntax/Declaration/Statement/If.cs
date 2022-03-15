using System;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class If : IStatement
{
    public Body Body { get; }
    public Body ElseBody { get; }
    public Expression.Expression Condition { get; }

    public static bool TryParse(TokenEnumerator tokens, out If? @if)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.If)
        {
            @if = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        // Try parse expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Declaration.Expression.Expression? expression))
        {
            throw new Exception($"Expression expected at line {tokens.Current().Position.Line}.");
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Then)
        {
            throw new Exception($"Keyword 'then' expected at line {tokens.Current().Position.Line}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        var body = new Body(tokens);

        // Else.
        Body? elseBody = new();
        // Keyword.
        if (tokens.Current() is Lexical.Tokens.Keywords.Else)
        {
            // Get next token.
            tokens.Next();
            // Try parse body.
            elseBody = new Body(tokens);
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Keyword 'end' expected at line {tokens.Current().Position.Line}.");
        }

        // Get next token.
        tokens.Next();

        @if = new If(expression!, body!, elseBody);
        return true;
    }

    private If(Expression.Expression condition, Body body, Body? elseBody = null)
    {
        Body = body;
        ElseBody = elseBody ?? new Body();
        Condition = condition;
    }
    
    public string ToString(string prefix)
    {
        var @string = new StringBuilder();
        
        @string.AppendLine($"if {Condition}");


        if (ElseBody is null)
        {
            @string.Append(Body.ToString(prefix));
            return @string.ToString();
        }
        
        @string.AppendLine(Body.ToString(prefix));
        
        // Else.
        @string.Append(prefix);
        @string.AppendLine("else");
        @string.Append(ElseBody.ToString(prefix));
        
        return @string.ToString();
    }
}
