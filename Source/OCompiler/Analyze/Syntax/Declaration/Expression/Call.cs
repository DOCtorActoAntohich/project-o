using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Analyze.Syntax.Declaration.Expression;

internal class Call : Expression
{
    public List<Expression> Arguments { get; }
    
    public Call(Token token, List<Expression> arguments, Expression? parent = null, Expression? child = null) : 
        base(token, child, parent)
    {
        Arguments = arguments;
    }
    
    public override string ToString()
    {
        return $"{Token.Literal}{Declaration.Expression.Arguments.ToString(Arguments)}";
    }
}
