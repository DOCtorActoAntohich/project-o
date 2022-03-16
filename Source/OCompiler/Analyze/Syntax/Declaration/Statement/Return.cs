using OCompiler.Analyze.Lexical;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Return : IStatement
{
    public Expression.Expression? ReturnValue { get; }
    public TokenPosition Position { get; }

    public static Return EmptyReturn => new(null, new());

    public static bool TryParse(TokenEnumerator tokens, out Return? @return)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Return)
        {
            @return = null;
            return false;
        }
        var position = tokens.Current().Position;
        // Get next token.
        tokens.Next();
        _ = Expression.Expression.TryParse(tokens, out Expression.Expression? expression);

        @return = new Return(expression, position);
        return true;
    }

    private Return(Expression.Expression? returnValue, TokenPosition position) {
        ReturnValue = returnValue;
        Position = position;
    }
    
    public string ToString(string _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"return {ReturnValue}";
    }
}
