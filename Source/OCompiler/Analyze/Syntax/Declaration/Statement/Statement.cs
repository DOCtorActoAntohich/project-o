using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Statement: INestable
{
    public Expression.Expression? Expression { get; }

    public static bool TryParse(IEnumerator<Token> tokens, out Statement? statement)
    {
        if (Assignment.TryParse(tokens, out Assignment? field))
        {
            statement = field;
            return true;
        }
        
        if (If.TryParse(tokens, out If? @if))
        {
            statement = @if;
            return true;
        }
        
        if (While.TryParse(tokens, out While? @while))
        {
            statement = @while;
            return true;
        }

        statement = null;
        return false;
    }
    
    protected Statement(Expression.Expression? expression)
    {
        Expression = expression;
    }
}
