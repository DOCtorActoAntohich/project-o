using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Assignment: Statement
{
    public Identifier Identifier { get; }
    
    public new Expression.Expression Expression { get; }
    
    public static Boolean TryParse(TokenEnumerator tokens, out Assignment? assignment)
    {
        // Parse name.
        if (tokens.Current() is not Identifier name)
        {
            assignment = null;
            return false;
        }
        
        // Assign delimiter.
        if (tokens.Next() is not Lexical.Tokens.Delimiters.Assign)
        {
            tokens.Back();
            assignment = null;
            return false;
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
    
    public override String ToString(String _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"{Identifier.Literal} := {Expression}";
    }
}
