using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal abstract class Expression: IBodyStatement
{
    public Expression? Parent { get; private set; }
    public Expression? Child { get; set; }
    public Token Token { get; }
    
    public static bool TryParse(TokenEnumerator tokens, out Expression? expression)
    {
        if (SimpleExpression.TryParse(tokens, out var simpleExpression))
        {
            expression = simpleExpression;
        }
        else if (ListDefinition.TryParse(tokens, out var listDefinition))
        {
            expression = listDefinition;
        }
        else if (DictDefinition.TryParse(tokens, out var dictDefinition))
        {
            expression = dictDefinition;
        }
        else
        {
            expression = null;
            return false;
        }

        // No dot. Only one expression.
        if (tokens.Current() is not Lexical.Tokens.Delimiters.Dot)
        {
            return true;
        }
        
        // Otherwise there are child expressions.
        // Get next token.
        tokens.Next();
        // Try parse child.
        if (!TryParse(tokens, out Expression? child))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected expression");
        }

        expression!.Child = child;
        child!.Parent = expression;

        return true;
    }

    public Expression(Token name, Expression? child = null, Expression? parent = null)
    {
        Parent = parent;
        Child = child;
        Token = name;
    }

    public string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        string @string = SelfToString();
        if (Child is not null)
        {
            @string += '.';
            @string += Child.ToString();
        }
        return @string;
    }

    protected abstract string SelfToString();
}
