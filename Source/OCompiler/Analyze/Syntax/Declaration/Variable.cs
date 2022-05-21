using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Exceptions;
using OCompiler.Utils;

using System;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Variable : IBodyStatement
{
    public Identifier Identifier { get; }
    public TypeAnnotation? Type { get; }
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


        // Optional type annotation.
        TypeAnnotation? type = null;
        if (tokens.Next() is Lexical.Tokens.Delimiters.Colon)
        {
            type = ParseTypeAnnotation(tokens);
        }

        // Assign delimiter.
        if (tokens.Current() is not Lexical.Tokens.Delimiters.Equals)
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

        variable = new Variable(name, type, expression!);
        return true;
    }

    private static TypeAnnotation? ParseTypeAnnotation(TokenEnumerator tokens)
    {
        tokens.Next();
        if (!TypeAnnotation.TryParse(tokens, out var type))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected a type");
        }
        return type;
    }

    protected Variable(Identifier name, TypeAnnotation? type, Expression.Expression expression)
    {
        Identifier = name;
        Type = type;
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
