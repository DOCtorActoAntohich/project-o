using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal class Expression: INestable
{
    public Expression? Parent { get; private set; }
    public Expression? Child { get; private set; }
    public Token Token { get; }
    
    public static Boolean TryParse(TokenEnumerator tokens, out Expression? expression)
    {
        if (tokens.Current() is not (Identifier or StringLiteral or RealLiteral or IntegerLiteral or BooleanLiteral))
        {
            expression = null;
            return false;
        }
        
        // Parse token
        Token token = tokens.Current();
        // Get next token.
        tokens.Next();
        
        // Try parse arguments.
        if (Arguments.TryParse(tokens, out List<Expression>? arguments))
        {
            expression = new Call(token, arguments!);
        }
        else
        {
            expression = new Expression(token);
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
            throw new Exception($"Expected expression at position {tokens.Current().StartOffset}.");
        }

        expression.Child = child;
        child!.Parent = expression;

        return true;
    }

    protected Expression(Token name, Expression? child = null, Expression? parent = null)
    {
        Parent = parent;
        Child = child;
        Token = name;
    }
    
    public String ToString(String _)
    {
        return ToString();
    }

    public override string ToString()
    {
        StringBuilder @string = new StringBuilder();

        if (Child is not null)
        {
            @string.Append('.');
            @string.Append(Child);
        }
        
        return $"{Token.Literal}{@string}";
    }
}
