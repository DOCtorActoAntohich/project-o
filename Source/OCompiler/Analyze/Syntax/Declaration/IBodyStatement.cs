using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal interface IBodyStatement
{
    public abstract string ToString(string prefix);

    public static bool TryParse(TokenEnumerator tokens, out IBodyStatement? bodyStatement)
    {
        if (Variable.TryParse(tokens, out Variable? variable))
        {
            bodyStatement = variable!;
            return true;
        }
        if (Statement.IStatement.TryParse(tokens, out Statement.IStatement? statement))
        {
            bodyStatement = statement!;
            return true;
        }
        if (Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            bodyStatement = expression!;
            return true;
        }

        bodyStatement = null;
        return false;
    }
}
