using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Exceptions;
using OCompiler.Utils;

using System;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Variable : IBodyStatement
{
    public Identifier Identifier { get; }
    public TypeAnnotation? Type { get; }
    public Expression.Expression? Expression { get; }

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

        // Optional assignment.
        Expression.Expression? expression = null;
        if (tokens.Current() is Lexical.Tokens.Delimiters.Equals)
        {
            expression = ParseAssignment(tokens);
        }

        variable = new Variable(name, type, expression);
        return true;
    }

    private static TypeAnnotation? ParseTypeAnnotation(TokenEnumerator tokens)
    {
        // Get next token.
        tokens.Next();

        // Type.
        if (!TypeAnnotation.TryParse(tokens, out var type))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected a type");
        }
        
        return type;
    }

    private static Expression.Expression? ParseAssignment(TokenEnumerator tokens)
    {
        // Get next token.
        tokens.Next();

        // Expression.
        if (!Declaration.Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected expression");
        }

        return expression;
    }

    protected Variable(Identifier name, TypeAnnotation? type, Expression.Expression? expression)
    {
        if (type is null && expression is null)
        {
            throw new SyntaxError(name.Position, "A variable must have either an assigned value or the type annotation");
        }

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
        var @string = $"var {Identifier.Literal}";
        if (Type is not null)
        {
            @string += $" : {Type}";
        }
        if (Expression is not null)
        {
            @string += $" = {Expression}";
        }

        return @string;
    }
}
