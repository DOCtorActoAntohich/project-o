using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal abstract class Statement: BodyStatement
{
    public Expression.Expression? Expression { get; }

    public static bool TryParse(TokenEnumerator tokens, out Statement? statement)
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
        
        if (Return.TryParse(tokens, out Return? @return))
        {
            statement = @return;
            return true;
        }
        
        statement = null;
        return false;
    }
    
    protected Statement(Expression.Expression? expression)
    {
        Expression = expression;
    }

    public override abstract string ToString(string prefix);
}
