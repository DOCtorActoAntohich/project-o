using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Assignment: Statement
{
    public Identifier Identifier { get; }
    
    public new Expression.Expression Expression { get; }
    
    public static Boolean TryParse(IEnumerator<Token> tokens, out Assignment? assignment)
    {
        // Parse name.
        if (tokens.Current() is not Identifier name)
        {
            throw new Exception($"Expected variable name at position {tokens.Current().StartOffset}.");
        }
        
        // Assign delimiter.
        if (tokens.Next() is not Lexical.Tokens.Delimiters.Assign)
        {
            throw new Exception($"Expected ':=' at position {tokens.Current().StartOffset}.");
        }

        // Get next token.
        tokens.Next();
        
        // Expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Declaration.Expression.Expression? expression))
        {
            throw new Exception($"Expected expression at position {tokens.Current().StartOffset}.");
        }
        
        assignment = new Assignment(name, expression!);
        return true;
    }
    
    protected Assignment(Identifier identifier, Expression.Expression expression) : base(expression)
    {
        Identifier = identifier;
        Expression = expression;
    }
}
