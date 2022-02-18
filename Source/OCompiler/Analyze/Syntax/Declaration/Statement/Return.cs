using System;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal class Return: Statement
{
    public static Boolean TryParse(TokenEnumerator tokens, out Return? @return)
    {
        // Keyword.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Return)
        {
            @return = null;
            return false;
        }
        
        // Get next token.
        tokens.Next();
        Declaration.Expression.Expression.TryParse(tokens, out Declaration.Expression.Expression? expression);

        @return = new Return(expression);
        return true;
    }

    private Return(Expression.Expression? expression): base(expression) { }
    
    public override String ToString(String _)
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"return {Expression}";
    }
}
