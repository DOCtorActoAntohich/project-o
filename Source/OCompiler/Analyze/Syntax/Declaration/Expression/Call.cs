using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal class Call : Expression
{
    public List<Expression> Arguments { get; }

    public static Call EmptyBaseCall = new(new Lexical.Tokens.Keywords.Base(), new());

    public Call(Token token, List<Expression> arguments, Expression? parent = null, Expression? child = null) :
        base(token, child, parent)
    {
        Arguments = arguments;
    }

    public Call(Expression expression) : base(expression.Token, expression.Child, expression.Parent)
    {
        // Create a Call without arguments from an existing Expression
        Arguments = new();
    }

    public override string ToString()
    {
        return $"{Token.Literal}{Declaration.Expression.Arguments.ToString(Arguments)}";
    }
}
