using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Variable : BodyStatement
{
    public Identifier Identifier { get; }
    public Expression.Expression Expression { get; }

    public static bool TryParse(TokenEnumerator tokens, out Variable? variable)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Var)
        {
            variable = null;
            return false;
        }

        // Parse name.
        if (tokens.Next() is not Identifier name)
        {
            throw new Exception($"Expected variable name at position {tokens.Current().StartOffset}.");
        }
        
        // Assign delimiter.
        if (tokens.Next() is not Lexical.Tokens.Delimiters.Colon)
        {
            throw new Exception($"Expected expression at position {tokens.Current().StartOffset}.");
        }

        // Get next token.
        tokens.Next();

        // Expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            throw new Exception($"Expected expression at position {tokens.Current().StartOffset}.");
        }

        variable = new Variable(name, expression!);
        return true;
    }

    protected Variable(Identifier name, Expression.Expression expression)
    {
        Identifier = name;
        Expression = expression;
    }

    public override string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"var {Identifier.Literal} : {Expression}";
    }
}
