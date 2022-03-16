using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal class Expression: IBodyStatement
{
    public Expression? Parent { get; private set; }
    public Expression? Child { get; set; }
    public Token Token { get; }
    
    public static bool TryParse(TokenEnumerator tokens, out Expression? expression)
    {
        if (tokens.Current() is not (
            Identifier or 
            StringLiteral or 
            RealLiteral or 
            IntegerLiteral or 
            BooleanLiteral or 
            Lexical.Tokens.Keywords.This or 
            Lexical.Tokens.Keywords.Base
        ))
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

    public Expression(Token name, Expression? child = null, Expression? parent = null)
    {
        Parent = parent;
        Child = child;
        Token = name;
    }

    public Call ReplaceWithCall()
    {
        var call = new Call(this);
        if (Parent != null)
        {
            Parent.Child = call;
        }
        return call;
    }

    public string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        string child = Child is null ? "" : $".{Child}";
        return $"{Token.Literal}{child}";
    }
}
