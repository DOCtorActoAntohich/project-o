using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Variable : IBodyStatement
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
            throw new SyntaxError(tokens.Current().Position, "Expected variable name");
        }
        
        // Assign delimiter.
        if (tokens.Next() is not Lexical.Tokens.Delimiters.Equals)
        {
            throw new SyntaxError(tokens.Current().Position, "Expected assignment");
        }

        // Get next token.
        tokens.Next();

        // Expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected expression");
        }

        variable = new Variable(name, expression!);
        return true;
    }

    protected Variable(Identifier name, Expression.Expression expression)
    {
        Identifier = name;
        Expression = expression;
    }

    public string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"var {Identifier.Literal} : {Expression}";
    }
}
