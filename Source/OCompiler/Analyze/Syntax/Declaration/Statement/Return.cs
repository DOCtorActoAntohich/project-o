using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Return : IStatement
{
    public Expression.Expression? ReturnValue { get; }

    public static bool TryParse(TokenEnumerator tokens, out Return? @return)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Return)
        {
            @return = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        _ = Expression.Expression.TryParse(tokens, out Expression.Expression? expression);

        @return = new Return(expression);
        return true;
    }

    private Return(Expression.Expression? returnValue) {
        ReturnValue = returnValue;
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
