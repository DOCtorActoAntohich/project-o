using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class If: Statement
{
    public List<INestable> Body { get; }
    public List<INestable>? ElseBody { get; }

    public static Boolean TryParse(TokenEnumerator tokens, out If? @if)
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
            throw new Exception($"Expression expected at position {tokens.Current().StartOffset}.");
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Then)
        {
            throw new Exception($"Keyword 'then' expected at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        if (!Declaration.Body.TryParse(tokens, out List<INestable>? body))
        {
            throw new Exception($"Body expected at position {tokens.Current().StartOffset}.");
        }
        
        // Else.
        List<INestable>? elseBody = null;
        // Keyword.
        if (tokens.Current() is Lexical.Tokens.Keywords.Else)
        {
            // Get next token.
            tokens.Next();
            // Try parse body.
            if (!Declaration.Body.TryParse(tokens, out elseBody))
            {
                throw new Exception($"Body expected at position {tokens.Current().StartOffset}.");
            }
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Keyword 'end' expected at position {tokens.Current().StartOffset}.");
        }

        @if = new If(expression!, body!, elseBody);
        return true;
    }

    private If(Expression.Expression expression, List<INestable> body, List<INestable>? elseBody = null) : 
        base(expression)
    {
        Body = body;
        ElseBody = elseBody;
    }
    
    public override String ToString(String prefix)
    {
        StringBuilder @string = new StringBuilder();
        
        @string.AppendLine($"if {Expression}");


        if (ElseBody is null)
        {
            @string.Append(Declaration.Body.ToString(Body, prefix));
            return @string.ToString();
        }
        
        @string.AppendLine(Declaration.Body.ToString(Body, prefix));
        
        // Else.
        @string.Append(prefix);
        @string.AppendLine("else");
        @string.Append(Declaration.Body.ToString(ElseBody, prefix));
        
        return @string.ToString();
    }
}
