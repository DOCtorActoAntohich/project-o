using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class While: Statement
{
    public List<INestable> Body { get; }
    
    public static Boolean TryParse(IEnumerator<Token> tokens, out While? @while)
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
        if (!Declaration.Expression.Expression.TryParse(tokens, out Declaration.Expression.Expression? expression))
        {
            throw new Exception($"Expression expected at position {tokens.Current().StartOffset}.");
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Loop)
        {
            throw new Exception($"Keyword 'loop' expected at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();
        // Try parse body.
        if (!Declaration.Body.TryParse(tokens, out List<INestable>? body))
        {
            throw new Exception($"Body expected at position {tokens.Current().StartOffset}.");
        }
        
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Keyword 'end' expected at position {tokens.Current().StartOffset}.");
        }

        @while = new While(expression!, body!);
        return true;
    }
    
    private While(Expression.Expression expression, List<INestable> body) : base(expression)
    {
        Body = body;
    }
}
